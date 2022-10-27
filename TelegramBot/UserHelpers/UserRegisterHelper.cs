using System;
using System.Threading;
using System.Threading.Tasks;
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

        public async ValueTask<Response> ProcessMessageAsync(string _message, CancellationToken _cancellationToken)
        {
            if (_message == Command)
            {
                if (await m_userManager.GetUserAsync(m_userId.ToString(), _cancellationToken) == null)
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
                if (int.TryParse(_message, out var id))
                {
                    m_waitingForId = false;
                    m_waitingForToken = true;

                    var url = m_vkApi.GetAuthUrl(id, VkApi.Requests.Permissions.Offline |
                                                     VkApi.Requests.Permissions.Wall |
                                                     VkApi.Requests.Permissions.Groups |
                                                     VkApi.Requests.Permissions.Friends |
                                                     VkApi.Requests.Permissions.Video);

                    return new Response(
                        $"Перейди по данной ссылке {url}, после чего пришли мне access_token из строки браузера");
                }

                return new Response("Id не распознан, попробуй еще раз");
            }

            if (m_waitingForToken)
            {
                if (!string.IsNullOrWhiteSpace(_message) && _message.Length > 60)
                {
                    m_waitingForToken = false;
                    await m_userManager.AddUserAsync(m_userId.ToString(), _message, _cancellationToken);

                    WorkCompleteEventHandler?.Invoke(m_userId);

                    return new Response("Успешно зарегистрировано! Вызови команду для добавления групп!");
                }

                return new Response("Токен не распознан как действительный, попробуй еще раз");
            }

            WorkCompleteEventHandler?.Invoke(m_userId);

            return null;
        }

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            m_vkApi = _vkApi ?? throw new ArgumentNullException(nameof(_vkApi));
            m_userManager = _userManager ?? throw new ArgumentNullException(nameof(_userManager));
            m_userId = _userId;
        }
    }
}
