using System;
using System.Collections.Generic;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Linq;
using VkGrabber;
using Telegram.Bot.Types.ReplyMarkups;
using VkApi;
using VkGrabber.Model;

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

        public Bot(string _token, IWebProxy _proxy = null)
        {
            m_telegramBot = new TelegramBotClient(_token, _proxy);
            m_registeredHelpers = new Dictionary<long, IUserHelper>();
            m_helperSelector = new BasicHelpersSelector(new List<IUserHelper>
            {
                new UserRegisterHelper(),
                new UserManagementHelper()
            });

            m_telegramBot.OnMessage += TelegramBot_OnMessage;
            m_telegramBot.OnUpdate += TelegramBot_OnUpdate;

            m_telegramBot.StartReceiving();

            m_userManager = new UserManager();
            m_grabber = new Grabber("5.92", TimeSpan.FromMinutes(15));
            m_grabber.Start();
            m_vkApi = new Vk("5.92");

            m_grabber.NewDataGrabbedEventHandler += Grabber_NewDataGrabbedEventHandler;
        }

        void TelegramBot_OnUpdate(object sender, UpdateEventArgs e)
        {

        }

        void Grabber_NewDataGrabbedEventHandler(string _userKey, Posts _posts)
        {
            foreach(var post in _posts)
            {
                var text = string.Format("Группа: {0} \n \n {1}", post.GroupName, post.Text);

                if (!post.Items.Any())
                    m_telegramBot.SendTextMessageAsync(int.Parse(_userKey), text);
                else
                {
                    if (post.Items.Length == 1)
                    {
                        switch (post.Items.First())
                        {
                            case ImageItem imageItem:
                                m_telegramBot.SendPhotoAsync(int.Parse(_userKey), new InputOnlineFile(imageItem.UrlMedium ?? imageItem.UrlSmall), text);
                                break;
                            case VideoItem videoItem:
                                m_telegramBot.SendVideoAsync(int.Parse(_userKey), new InputOnlineFile(videoItem.Url), caption: text);
                                break;
                        }
                    }
                    else
                    {
                        m_telegramBot.SendTextMessageAsync(int.Parse(_userKey), text);

                        var list = new List<IAlbumInputMedia>();

                        foreach (var postItem in post.Items)
                        {
                            switch (postItem)
                            {
                                case ImageItem imageItem:
                                    list.Add(new InputMediaPhoto(new InputMedia(imageItem.UrlMedium ?? imageItem.UrlSmall)));
                                    break;
                                case VideoItem videoItem:
                                    if (string.IsNullOrEmpty(videoItem.Url))
                                        list.Add(new InputMediaVideo(new InputMedia(videoItem.Url)));
                                    break;
                            }
                        }
                        if (list.Any())
                            m_telegramBot.SendMediaGroupAsync(list, int.Parse(_userKey));
                    }
                }
            }
        }

        private void TelegramBot_OnMessage(object _sender, MessageEventArgs _e)
        {
            Console.WriteLine(_e.Message.Chat.Id);

            var message = _e.Message.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(message))
            {
                if (message.StartsWith("/", StringComparison.Ordinal))
                {
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
                }

                if (m_registeredHelpers.ContainsKey(_e.Message.Chat.Id))
                {
                    var helper = m_registeredHelpers[_e.Message.Chat.Id];

                    var response = helper.OnMessage(_e.Message.Text);

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

        internal InlineKeyboardButton[][] GetInlineKeyboard(string[] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[1][];
            var keyboardButtons = new InlineKeyboardButton[stringArray.Length];
            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton
                {
                    Text = stringArray[i],
                    CallbackData = "some data"
                };
            }
            keyboardInline[0] = keyboardButtons;
            return keyboardInline;
        }

        public class CallBackData
        {
            public string Operation { get; }
            public int MessageId { get; }
        }
    }
}
