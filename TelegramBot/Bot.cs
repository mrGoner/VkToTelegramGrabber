using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VkGrabber;
using VkApi;
using VkGrabber.Model;
using TelegramBot.UserHelpers;
using Telegram.Bot.Types.Enums;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using TelegramBot.Queue;
using Message = TelegramBot.Queue.Message;

namespace TelegramBot;

public class Bot : IDisposable
{
    private readonly TelegramBotClient _telegramBot;
    private readonly UserManager _userManager;
    private readonly Grabber _grabber;
    private readonly Vk _vkApi;
    private readonly ConcurrentDictionary<long, IUserHelper> _registeredHelpers;
    private readonly IUserHelperSelector _helperSelector;
    private string? _botName;
    private readonly MessageQueue _messageQueue;
    private readonly ILogger<Bot> _logger;

    public Bot(string token, string pathToDb, string logsDir, IUserHelperSelector? helperSelector = null,
        HttpClient? proxy = null)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));
        
        var loggerFactory = new NLogLoggerFactory();

        LogManager.Configuration?.Variables["logs_dir"] = logsDir;

        _telegramBot = new TelegramBotClient(token, proxy);
        _registeredHelpers = new ConcurrentDictionary<long, IUserHelper>();
        _helperSelector = helperSelector ??
                          new BasicHelpersSelector(
                              new Dictionary<string, Type>
                              {
                                  { UserRegisterHelper.Command, typeof(UserRegisterHelper) },
                                  { UserManagementHelper.Command, typeof(UserManagementHelper) },
                                  { InfoHelper.Command, typeof(InfoHelper) }
                              }, new DefaultHelper(), loggerFactory);

        _userManager = new UserManager(pathToDb);

        _grabber = new Grabber(TimeSpan.FromMinutes(1), 20, 1000, pathToDb, loggerFactory.CreateLogger<Grabber>());

        _vkApi = new Vk();

        _messageQueue = new MessageQueue(TimeSpan.FromSeconds(10), 4, SendMessage, 3,
            loggerFactory.CreateLogger<MessageQueue>());
       
        _logger = loggerFactory.CreateLogger<Bot>();

        _grabber.NewDataGrabbedEventHandler += Grabber_NewDataGrabbedEventHandler;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        try
        {
            _botName = (await _telegramBot.GetMe(cancellationToken)).Username ??
                       throw new InvalidOperationException("Failed to get bot name");

            _logger.LogInformation("Bot name: {BotName} started", _botName);

            await _grabber.Start(cancellationToken);

            _telegramBot.StartReceiving(UpdateHandler, PollingErrorHandler, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start bot");
        }
    }

    private Task PollingErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Pooling error occured");
        return Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message) 
            await HandleMessage(update.Message, cancellationToken);
    }

    private async Task HandleMessage(Telegram.Bot.Types.Message? message, CancellationToken cancellationToken)
    {
        if (message is null)
            return;
        
        var messageText = message.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(messageText))
        {
            if (messageText.StartsWith('/'))
            {
                if (messageText.Contains($"@{_botName}"))
                    messageText = messageText.Replace($"@{_botName}", "");

                if (_registeredHelpers.TryGetValue(message.Chat.Id, out var helper))
                {
                    helper.WorkCompleteEventHandler -= OnHelperWorkDone;

                    _registeredHelpers.TryRemove(message.Chat.Id, out _);
                }

                if (_helperSelector.TryGetCompatibleHelper(messageText, out var newHelper))
                {
                    newHelper!.Init(message.Chat.Id, _vkApi, _userManager);
                    newHelper.WorkCompleteEventHandler += OnHelperWorkDone;
                    _registeredHelpers.TryAdd(message.Chat.Id, newHelper);
                }
                else
                {
                    var replyMessage = _helperSelector.DefaultHelper.GetDefaultResponse().ReplyMessage;
                    await _telegramBot.SendMessage(message.Chat.Id, replyMessage, cancellationToken: cancellationToken);

                    return;
                }
            }

            if (_registeredHelpers.ContainsKey(message.Chat.Id))
            {
                var helper = _registeredHelpers[message.Chat.Id];

                var response = await helper.ProcessMessageAsync(messageText, cancellationToken);

                if (response == null)
                {
                    _registeredHelpers.TryRemove(message.Chat.Id, out _);
                }
                else
                {
                    await _telegramBot.SendMessage(message.Chat.Id, response.ReplyMessage,
                        replyMarkup: response.ReplyMarkup, cancellationToken: cancellationToken);
                }
            }
        }
    }

    public void Stop()
    {
        _grabber.Stop();
    }

    private void Grabber_NewDataGrabbedEventHandler(string userKey, Posts posts)
    {
        foreach (var post in posts) 
            _messageQueue.AddMessage(new Message(long.Parse(userKey), post));
    }

    private async Task SendMessage(long userId, Post post)
    {
        if (post is null)
            throw new ArgumentNullException(nameof(post));

        var text = $"Группа: {post.GroupName} \n \n {post.Text}";

        try
        {
            if (!post.Items.Any())
            {
                await SendMessage(userId, text);
            }
            else
            {
                if (post.Items.Length == 1)
                {
                    if (text.Length > 1020)
                    {
                        _logger.LogInformation(
                            $"Post have text with length: {text.Length}. Send media and text separately");

                        await SendMessage(userId, text);
                        text = string.Empty;
                    }

                    switch (post.Items.First())
                    {
                        case AudioItem audioItem:
                            await _telegramBot.SendAudio(userId, new InputFileUrl(audioItem.Url), text);
                            break;
                        case ImageItem imageItem:
                            await _telegramBot.SendPhoto(userId, new InputFileUrl(imageItem.UrlLarge ?? imageItem.UrlMedium ?? imageItem.UrlSmall), text);
                            break;
                        case VideoItem videoItem:
                            await _telegramBot.SendMessage(userId, $"{text}\n{videoItem.Url}");
                            break;
                        case DocumentItem documentItem:
                            await _telegramBot.SendDocument(userId, new InputFileUrl(documentItem.Url), $"{text}\n{documentItem.Title}");
                            break;
                        case LinkItem linkItem:
                            await _telegramBot.SendMessage(userId, $"{text}\n{linkItem.Url}");
                            break;
                        case NoteItem noteItem:
                            await _telegramBot.SendMessage(userId, $"{text}\n{noteItem.Text}");
                            break;
                    }
                }
                else
                {
                    await SendMessage(userId, text);

                    var mediaToSend = new List<(MediaType MediaType, IAlbumInputMedia Media)>(post.Items.Length);

                    foreach (var postItem in post.Items)
                    {
                        switch (postItem)
                        {
                            case DocumentItem documentItem:
                                mediaToSend.Add((MediaType.Document, new InputMediaDocument(new InputFileUrl(documentItem.Url))));
                                break;
                            case ImageItem imageItem:
                                mediaToSend.Add((MediaType.Image, new InputMediaPhoto(new InputFileUrl(imageItem.UrlLarge ?? imageItem.UrlMedium ?? imageItem.UrlSmall))));
                                break;
                            case VideoItem videoItem:
                                mediaToSend.Add((MediaType.Video, new InputMediaVideo(new InputFileUrl(videoItem.Url))));
                                break;
                        }
                    }

                    foreach (var mediaGroup in mediaToSend.GroupBy(tuple => tuple.MediaType, tuple => tuple.Media))
                        await _telegramBot.SendMediaGroup(userId, mediaGroup);
                }
            }
        }
        catch (Exception ex) when (ex.Message.Contains("WEBPAGE_MEDIA_EMPTY"))
        {
            _logger.LogWarning(ex, "Media not available, skip it. Post: {Post}", post);
        }
    }

    private async Task SendMessage(long userId, string message)
    {
        if(message.Length > 4090)
            _logger.LogWarning($"Text length is {message.Length} chunk it");

        foreach (var chunkedText in message.Chunk(4090))
            await _telegramBot.SendMessage(userId, new string(chunkedText));
    }

    private void OnHelperWorkDone(long userKey)
    {
        if (_registeredHelpers.TryRemove(userKey, out var removedHelper))
            removedHelper.WorkCompleteEventHandler -= OnHelperWorkDone;
    }

    public void Dispose()
    {
        _grabber.NewDataGrabbedEventHandler -= Grabber_NewDataGrabbedEventHandler;
        _grabber.Dispose();
    }
    
    private enum MediaType
    {
        Document,
        Image,
        Video
    }
}