using System;
using VkGrabber;
using VkApi;

namespace TelegramBot.UserHelpers
{
    public class UserRegisterHelper : IUserHelper
    {
        private bool m_waitingForId;
        private bool m_waitingForToken;
        private Vk m_vkApi;
        private UserManager m_userManager;
        private long m_userId;

        public string Command => "/register";

        public event WorkComplete WorkCompleteEventHandler;

        public Response OnCallBackUpdate()
        {
            return null;
        }

        public Response OnMessage(string _message)
        {
            if (_message == Command)
            {
                if (m_userManager.GetUser(m_userId.ToString()) == null)
                {
                    m_waitingForId = true;
                    return new Response("Для регистрации необходимо выполнить следующие шаги: \n" +
                                        "1) Перейти по ссылке https://vk.com/apps?act=manage и создать новое standalone приложение \n" +
                                        "2) Прислать id приложения (находится в настройках созданного приложения) \n" +
                                        "3) В ответ я сгенерирую ссылку, перейдя по которой необходимо будет прислать мне access token \n \n" +
                                        "Кажется сложным? Исходники бота можно найти по ссылке https://github.com/mrGoner/VkToTelegramGrabber и развернуть своего собственного! \n" +
                                        "Если же готов продолжить - присылай id созданного приложения");
                }

                WorkCompleteEventHandler?.Invoke(m_userId);

                return new Response("Ты уже зарегистрирован, если хочешь ввести другой токен - необходимо удалить текущую учетную запись");
            }

            if (m_waitingForId)
            {
                m_waitingForId = false;

                if (int.TryParse(_message, out var id))
                {
                    m_waitingForToken = true;

                    var url = m_vkApi.GetAuthUrl(id, VkApi.Requests.Permissions.Offline |
                                                     VkApi.Requests.Permissions.Wall |
                                                     VkApi.Requests.Permissions.Groups |
                                                     VkApi.Requests.Permissions.Friends);

                    return new Response($"Перейди по данной ссылке {url}, после чего пришли мне access_token из строки браузера");
                }

                return new Response("Id не распознан");
            }

            if (m_waitingForToken)
            {
                m_waitingForToken = false;

                if (!string.IsNullOrWhiteSpace(_message) && _message.Length > 60)
                {
                    m_waitingForToken = false;
                    m_userManager.AddUser(m_userId.ToString(), _message);

                    WorkCompleteEventHandler?.Invoke(m_userId);

                    return new Response("Успешно зарегистрировано!");
                }

                return new Response("Токен не распознан как действительный");
            }

            WorkCompleteEventHandler?.Invoke(m_userId);

            return null;
        }

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            m_vkApi = _vkApi ?? throw new ArgumentNullException(nameof(_vkApi));
            m_userManager = _userManager ?? throw new ArgumentNullException(nameof(_vkApi));
            m_userId = _userId;
        }
    }
}
