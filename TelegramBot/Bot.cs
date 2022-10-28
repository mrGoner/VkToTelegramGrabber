using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VkGrabber;
using VkApi;
using VkGrabber.Model;
using TelegramBot.UserHelpers;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Helpers;
using Telegram.Bot.Types.Enums;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace TelegramBot
{
    public class Bot : IDisposable
    {
        private readonly TelegramBotClient m_telegramBot;
        private readonly UserManager m_userManager;
        private readonly Grabber m_grabber;
        private readonly Vk m_vkApi;
        private readonly ConcurrentDictionary<long, IUserHelper> m_registeredHelpers;
        private readonly IUserHelperSelector m_helperSelector;
        private string m_botName;
        private readonly MessageQueue m_messageQueue;
        private readonly ILogger m_logger;

        public Bot(string _token, IUserHelperSelector _helperSelector = null, HttpClient _proxy = null)
        {
            if (_token == null) 
                throw new ArgumentNullException(nameof(_token));

            m_telegramBot = new TelegramBotClient(_token, _proxy);
            m_registeredHelpers = new ConcurrentDictionary<long, IUserHelper>();
            m_helperSelector = _helperSelector ??
                new BasicHelpersSelector(new List<IUserHelper>
            {
                new UserRegisterHelper(),
                new UserManagementHelper(),
                new InfoHelper()
            },
            new DefaultHelper());

            m_userManager = new UserManager();

            var loggerFactory = new NLogLoggerFactory();

            m_grabber = new Grabber(TimeSpan.FromMinutes(1), 20, 1000, loggerFactory);
            
            m_vkApi = new Vk();
            m_messageQueue = new MessageQueue(TimeSpan.FromSeconds(10), 4, SendMessage);
            m_logger = loggerFactory.CreateLogger(typeof(Bot));
            
            m_grabber.NewDataGrabbedEventHandler += Grabber_NewDataGrabbedEventHandler;
        }

        public async Task Start(CancellationToken _cancellationToken)
        {
            await m_grabber.Start(_cancellationToken);
            m_botName = (await m_telegramBot.GetMeAsync(_cancellationToken)).Username;
            
            m_telegramBot.StartReceiving(UpdateHandler, PollingErrorHandler, cancellationToken: _cancellationToken);
            
            m_logger.LogDebug("Bot name: {BotName} started", m_botName);
        }

        private Task PollingErrorHandler(ITelegramBotClient _client, Exception _exception, CancellationToken _cancellationToken)
        {
            m_logger.LogError(_exception, "Pooling error occured");
            return Task.CompletedTask;
        }

        private async Task UpdateHandler(ITelegramBotClient _client, Update _update, CancellationToken _cancellationToken)
        {
            switch (_update.Type)
            {
                case UpdateType.Message:
                    await HandleMessage(_update.Message, _cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await HandleCallbackQueryAsync(_update.CallbackQuery, _cancellationToken);
                    break;
            }
        }

        private async Task HandleMessage(Telegram.Bot.Types.Message _message, CancellationToken _cancellationToken)
        {
            var message = _message.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(message))
            {
                if (message.StartsWith("/", StringComparison.Ordinal))
                {
                    if (message.Contains($"@{m_botName}"))
                        message = message.Replace($"@{m_botName}", "");

                    if (m_registeredHelpers.TryGetValue(_message.Chat.Id, out var helper))
                    {
                        helper.WorkCompleteEventHandler -= OnHelperWorkDone;

                        m_registeredHelpers.TryRemove(_message.Chat.Id, out _);
                    }

                    if (m_helperSelector.TryGetCompatibleHelper(message, out var newHelper))
                    {
                        newHelper.Init(_message.Chat.Id, m_vkApi, m_userManager);
                        newHelper.WorkCompleteEventHandler += OnHelperWorkDone;
                        m_registeredHelpers.TryAdd(_message.Chat.Id, newHelper);
                    }
                    else
                    {
                        await m_telegramBot.SendTextMessageAsync(_message.Chat.Id,
                            m_helperSelector.DefaultHelper.GetDefaultResponce().ReplyMessage,
                            cancellationToken: _cancellationToken);

                        return;
                    }
                }

                if (m_registeredHelpers.ContainsKey(_message.Chat.Id))
                {
                    var helper = m_registeredHelpers[_message.Chat.Id];

                    var response = await helper.ProcessMessageAsync(message, _cancellationToken);

                    if (response == null)
                    {
                        m_registeredHelpers.TryRemove(_message.Chat.Id, out _);
                    }
                    else
                    {
                        await m_telegramBot.SendTextMessageAsync(_message.Chat.Id, response.ReplyMessage,
                            replyMarkup: response.ReplyMarkup, cancellationToken: _cancellationToken);
                    }
                }
            }
        }

        public void Stop()
        {
            m_grabber.Stop();
        }

        private async Task HandleCallbackQueryAsync(CallbackQuery _callbackQuery, CancellationToken _cancellationToken)
        {
            if(_callbackQuery?.Message == null)
                return;
            
            if (_callbackQuery.Data == null)
                return;

            if (PostLikeHelper.TryParseLikeInfo(_callbackQuery.Data, out var likeInfo))
            {
                try
                {
                    if (likeInfo.IsLiked)
                    {
                        await m_telegramBot.AnswerCallbackQueryAsync(_callbackQuery.Id,
                            text: "Отметка ❤ уже поставлена", cancellationToken: _cancellationToken);

                        return;
                    }

                    var user = await m_userManager.GetUserAsync(_callbackQuery.Message.Chat.Id.ToString(), _cancellationToken);

                    if (user == null)
                    {
                        await m_telegramBot.AnswerCallbackQueryAsync(_callbackQuery.Id, text: "Пользователь не найден",
                            cancellationToken: _cancellationToken);

                        return;
                    }

                    try
                    {
                        await m_telegramBot.AnswerCallbackQueryAsync(_callbackQuery.Id, text: "Вам ❤ это",
                            cancellationToken: _cancellationToken);

                        likeInfo.IsLiked = true;

                        var likeButton = KeyBoardBuilder.BuildInlineKeyboard(new[]
                        {
                            new KeyValuePair<string, string>("✅❤", PostLikeHelper.SerializeInfo(likeInfo))
                        }) as InlineKeyboardMarkup;

                        await m_telegramBot.EditMessageReplyMarkupAsync(_callbackQuery.Message.Chat.Id,
                            _callbackQuery.Message.MessageId, replyMarkup: likeButton,
                            cancellationToken: _cancellationToken);

                        await m_vkApi.LikePostAsync(likeInfo.OwnerId, (uint)likeInfo.ItemId, user.Token, _cancellationToken);
                    }
                    catch(Exception ex)
                    {
                        likeInfo.IsLiked = false;

                        await m_telegramBot.AnswerCallbackQueryAsync(_callbackQuery.Id, text: "Не удалось поставить ❤",
                            cancellationToken: _cancellationToken);

                        var likeButton = KeyBoardBuilder.BuildInlineKeyboard(new[]
                        {
                            new KeyValuePair<string, string>("❤", PostLikeHelper.SerializeInfo(likeInfo))
                        }) as InlineKeyboardMarkup;

                        await m_telegramBot.EditMessageReplyMarkupAsync(_callbackQuery.Message.Chat.Id,
                            _callbackQuery.Message.MessageId, replyMarkup: likeButton,
                            cancellationToken: _cancellationToken);

                        m_logger.LogError(ex, "Failed to answer likes");
                    }
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "Error while HandleCallbackQueryAsync");
                }
            }
        }

        private void Grabber_NewDataGrabbedEventHandler(string _userKey, Posts _posts)
        {
            foreach(var post in _posts)
            {
                m_messageQueue.AddMessage(new Message(long.Parse(_userKey), post));
            }
        }

        private async Task SendMessage(long _userId, Post _post)
        {
            if (_post is null)
                throw new ArgumentNullException(nameof(_post));

            var text = $"Группа: {_post.GroupName} \n \n {_post.Text}";

            var serializedLikeInfo = PostLikeHelper.SerializeInfo(new LikeInfo
            {
                OwnerId = -_post.GroupId,
                ItemId  = _post.PostId
            });

            var likeButton = KeyBoardBuilder.BuildInlineKeyboard(new[] { new KeyValuePair<string, string>("❤", serializedLikeInfo) });

            if (!_post.Items.Any())
                await m_telegramBot.SendTextMessageAsync(_userId, text, replyMarkup: likeButton);
            else
            {
                if (_post.Items.Length == 1)
                {
                    switch (_post.Items.First())
                    {
                        case ImageItem imageItem:
                            await m_telegramBot.SendPhotoAsync(_userId,
                                new InputOnlineFile(imageItem.UrlMedium ?? imageItem.UrlLarge), text, replyMarkup: likeButton);
                            break;
                        case VideoItem videoItem:
                            if (!string.IsNullOrEmpty(videoItem.Url))
                                await m_telegramBot.SendTextMessageAsync(_userId, $"{text}\n{videoItem.Url}", replyMarkup: likeButton);
                            break;
                        case DocumentItem documentItem:
                            await m_telegramBot.SendDocumentAsync(_userId,
                                new InputOnlineFile(documentItem.Url), caption:$"{text}\n{documentItem.Title}", replyMarkup: likeButton);
                            break;
                        case LinkItem linkItem:
                            await m_telegramBot.SendTextMessageAsync(_userId, $"{text}\n{linkItem.Url}", replyMarkup: likeButton);
                            break;
                    }
                }
                else
                {
                    await m_telegramBot.SendTextMessageAsync(_userId, text, replyMarkup: likeButton);

                    var media = new List<IAlbumInputMedia>();

                    foreach (var postItem in _post.Items)
                    {
                        switch (postItem)
                        {
                            case ImageItem imageItem:
                                media.Add(new InputMediaPhoto(
                                    new InputMedia(imageItem.UrlLarge ?? imageItem.UrlMedium ?? imageItem.UrlSmall)));
                                break;
                            case VideoItem videoItem:
                                if (!string.IsNullOrEmpty(videoItem.Url))
                                    media.Add(new InputMediaVideo(new InputMedia(videoItem.Url)));
                                break;
                        }
                    }

                    if (media.Any())
                        await m_telegramBot.SendMediaGroupAsync(_userId, media);
                }
            }
        }

        private void OnHelperWorkDone(long _userKey)
        {
            var helper = m_registeredHelpers.FirstOrDefault(_x => _x.Key == _userKey);

            m_registeredHelpers.TryRemove(helper.Key, out _);
        }

        public void Dispose()
        {
            m_grabber.NewDataGrabbedEventHandler -= Grabber_NewDataGrabbedEventHandler;
            m_grabber.Dispose();
        }
    }
}