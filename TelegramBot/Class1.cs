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
        private readonly Dictionary<long, HelperBase> m_registeredHelpers;

        public Bot(string _token, IWebProxy _proxy = null)
        {
            m_telegramBot = new TelegramBotClient(_token, _proxy);
            m_registeredHelpers = new Dictionary<long, HelperBase>();

            m_telegramBot.OnMessage += TelegramBot_OnMessage;
            m_telegramBot.OnUpdate += TelegramBot_OnUpdate;

            m_telegramBot.StartReceiving();

            m_userManager = new UserManager();
            m_grabber = new Grabber("5.92", TimeSpan.FromMinutes(1));
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
            /*
            if (_e.Message.Text.Trim() == "/help" || _e.Message.Text.Trim() == "/start")
            {
            }
            else if (_e.Message.Text.Trim().StartsWith("/register", StringComparison.Ordinal))
            {
                if (m_registeredHelpers.TryGetValue(_e.Message.Chat.Id, out var helper))
                {
                    helper.WorkCompleteEventHandler -= OnHelperWorkDone;

                    m_registeredHelpers.Remove(_e.Message.Chat.Id);
                }

                var registerHelper = new UserRegisterHelper(_e.Message.Chat.Id, m_vkApi, m_userManager);
                registerHelper.WorkCompleteEventHandler += OnHelperWorkDone;
                m_registeredHelpers.Add(_e.Message.Chat.Id, registerHelper);
            }

            if (m_registeredHelpers.ContainsKey(_e.Message.Chat.Id))
            {
                var helper = m_registeredHelpers[_e.Message.Chat.Id];

                if (helper != null)
                {
                    var response = helper.OnMessage(_e.Message.Text);

                    if (response != null)
                        m_telegramBot.SendTextMessageAsync(_e.Message.Chat.Id, response.ReplyMessage, replyMarkup: response.ReplyMarkup);
                }
            }*/
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

        public class UserRegisterHelper : HelperBase
        {
            private bool m_waitingForId;
            private bool m_waitingForToken;
            private readonly string m_command = "/register";
            private readonly Vk m_vkApi;
            private readonly UserManager m_userManager;
            private readonly long m_userId;

            public override event WorkComplete WorkCompleteEventHandler;

            public UserRegisterHelper(long _userId, Vk _vkApi, UserManager _userManager)
            {
                m_vkApi = _vkApi ?? throw new ArgumentNullException(nameof(_vkApi));
                m_userManager = _userManager ?? throw new ArgumentNullException(nameof(_vkApi));
                m_userId = _userId;
            }

            public override Response OnCallBackUpdate()
            {
                return null;
            }

            public override Response OnMessage(string _message)
            {
                if(_message.StartsWith(m_command, StringComparison.Ordinal))
                {
                    m_waitingForId = true;
                    return new Response("Для регистрации необходимо выполнить следующие шаги: \n" +
                                        "1) Перейти по ссылке https://vk.com/apps?act=manage и создать новое standalone приложение \n" +
                                        "2) Прислать id приложения (находится в настройках созданного приложения) \n" +
                                        "3) В ответ я сгенерирую ссылку, перейдя по которой необходимо будет прислать мне access token \n \n" +
                                        "Кажется сложным? Исходники бота можно найти по ссылке %ссылка будет когда допилю% и развернуть своего собственного! \n" +
                                        "Если же готов продолжить - присылай id созданного приложения");
                }

                if (m_waitingForId)
                {
                    if (int.TryParse(_message, out var id))
                    {
                        m_waitingForId = false;
                        m_waitingForToken = true;

                        var url = m_vkApi.GetAuthUrl(id, VkApi.Requests.Permissions.Offline |
                                                         VkApi.Requests.Permissions.Wall |
                                                         VkApi.Requests.Permissions.Groups |
                                                         VkApi.Requests.Permissions.Friends);

                        return new Response($"Перейди по данной ссылке {url}, после чего пришли мне access_token из строки браузера");
                    }

                    return new Response("Id не распознан( Попробуй снова (никит-кикит)!");
                }

                if (m_waitingForToken)
                {
                    if (!string.IsNullOrWhiteSpace(_message) && _message.Length > 60)
                    {
                        m_waitingForToken = false;
                        m_userManager.AddUser(m_userId.ToString(), _message);

                        WorkCompleteEventHandler?.Invoke(m_userId);

                        return new Response("Успешно зарегистрировано!");
                    }

                    return new Response("Токен не распознан как действительный, попробуй еще");
                }

                WorkCompleteEventHandler?.Invoke(m_userId);

                return null;
            }
        }

        public class UserManagementHelper
        {

        }

        public abstract class HelperBase
        {
            public delegate void WorkComplete(long _key);
            public abstract event WorkComplete WorkCompleteEventHandler;

            public abstract Response OnCallBackUpdate();
            public abstract Response OnMessage(string _message);
        }

        public class Response
        {
            public IReplyMarkup ReplyMarkup { get; }
            public string ReplyMessage { get; }

            public Response(string _replyMessage)
            {
                ReplyMessage = _replyMessage;
            }

            public Response(string _replyMessage, IReplyMarkup _replyMarkup)
            {
                ReplyMessage = _replyMessage;
                ReplyMarkup = _replyMarkup;
            }
        }
    }
}
