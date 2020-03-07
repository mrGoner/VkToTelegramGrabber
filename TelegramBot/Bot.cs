﻿using System;
using System.Collections.Generic;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Linq;
using VkGrabber;
using VkApi;
using VkGrabber.Model;
using TelegramBot.UserHelpers;

namespace TelegramBot
{
    public class Bot
    {
        private readonly TelegramBotClient m_telegramBot;
        public readonly UserManager m_userManager;
        private readonly Grabber m_grabber;
        private readonly Vk m_vkApi;
        private readonly Dictionary<long, IUserHelper> m_registeredHelpers;
        private readonly IUserHelperSelector m_helperSelector;
        private readonly string m_botName;
        private const string m_apiVersion = "5.103";
        private readonly MessageQueue m_messageQueue;

        public Bot(string _token, IUserHelperSelector _helperSelector, IWebProxy _proxy = null)
        {
            m_telegramBot = new TelegramBotClient(_token, _proxy);
            m_registeredHelpers = new Dictionary<long, IUserHelper>();
            m_helperSelector = _helperSelector ??
                new BasicHelpersSelector(new List<IUserHelper>
            {
                new UserRegisterHelper(),
                new UserManagementHelper(),
                new InfoHelper()
            },
            new DefaultHelper());

            m_telegramBot.OnMessage += TelegramBot_OnMessage;

            m_userManager = new UserManager();
            m_grabber = new Grabber(m_apiVersion, TimeSpan.FromMinutes(15));
            m_grabber.Start();
            m_vkApi = new Vk(m_apiVersion);
            m_messageQueue = new MessageQueue(TimeSpan.FromSeconds(7), 4, SendMessage);

            m_grabber.NewDataGrabbedEventHandler += Grabber_NewDataGrabbedEventHandler;

            m_botName = m_telegramBot.GetMeAsync().Result.Username;

            Console.WriteLine($"Bot name: {m_botName}");

            m_telegramBot.StartReceiving();
        }

        private void Grabber_NewDataGrabbedEventHandler(string _userKey, Posts _posts)
        {
            foreach(var post in _posts)
            {
                m_messageQueue.AddMessage(new Message(long.Parse(_userKey), post));
            }
        }

        private void SendMessage(long _userId, Post _post)
        {
            if (_post is null)
                throw new ArgumentNullException(nameof(_post));

            var text = string.Format("Группа: {0} \n \n {1}", _post.GroupName, _post.Text);

            if (!_post.Items.Any())
                m_telegramBot.SendTextMessageAsync(_userId, text);
            else
            {
                if (_post.Items.Length == 1)
                {
                    switch (_post.Items.First())
                    {
                        case ImageItem imageItem:
                            m_telegramBot.SendPhotoAsync(_userId, new InputOnlineFile(imageItem.UrlMedium ?? imageItem.UrlSmall), text);
                            break;
                        case VideoItem videoItem:
                            m_telegramBot.SendTextMessageAsync(_userId, $"{text}\n{videoItem.Url}");
                            break;
                        case DocumentItem documentItem:
                            m_telegramBot.SendDocumentAsync(_userId, new InputOnlineFile(documentItem.Url), $"{text}\n{documentItem.Title}");
                            break;
                        case LinkItem linkItem:
                            m_telegramBot.SendTextMessageAsync(_userId, $"{text}\n{linkItem.Url}");
                            break;
                    }
                }
                else
                {
                    m_telegramBot.SendTextMessageAsync(_userId, text);

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
                                if (string.IsNullOrEmpty(videoItem.Url))
                                    media.Add(new InputMediaVideo(new InputMedia(videoItem.Url)));
                                break;
                        }
                    }
                    if (media.Any())
                        m_telegramBot.SendMediaGroupAsync(media, _userId);
                }
            }
        }

        private void TelegramBot_OnMessage(object _sender, MessageEventArgs _e)
        {
            var message = _e.Message.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(message))
            {
                if (message.StartsWith("/", StringComparison.Ordinal))
                {
                    if (message.Contains($"@{m_botName}"))
                        message = message.Replace($"@{m_botName}", "");

                    if (m_registeredHelpers.TryGetValue(_e.Message.Chat.Id, out var helper))
                    {
                        helper.WorkCompleteEventHandler -= OnHelperWorkDone;

                        m_registeredHelpers.Remove(_e.Message.Chat.Id);
                    }

                    if (m_helperSelector.TryGetCompatibleHelper(message, out var newHelper))
                    {
                        newHelper.Init(_e.Message.Chat.Id, m_vkApi, m_userManager);
                        newHelper.WorkCompleteEventHandler += OnHelperWorkDone;
                        m_registeredHelpers.Add(_e.Message.Chat.Id, newHelper);
                    }
                    else
                    {
                        m_telegramBot.SendTextMessageAsync(_e.Message.Chat.Id,
                            m_helperSelector.DefaultHelper.GetDefaultResponce().ReplyMessage);

                        return;
                    }
                }

                if (m_registeredHelpers.ContainsKey(_e.Message.Chat.Id))
                {
                    var helper = m_registeredHelpers[_e.Message.Chat.Id];

                    var response = helper.OnMessage(message);

                    if (response != null)
                        m_telegramBot.SendTextMessageAsync(_e.Message.Chat.Id, response.ReplyMessage, replyMarkup: response.ReplyMarkup);
                }
            }
        }

        private void OnHelperWorkDone(long _userKey)
        {
            var helper = m_registeredHelpers.FirstOrDefault(_x => _x.Key == _userKey);

            m_registeredHelpers.Remove(helper.Key);
        }
    }
}