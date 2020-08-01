using VkApi;
using VkGrabber;

namespace TelegramBot.UserHelpers
{
    public class InfoHelper : IUserHelper
    {
        public string Command => "/help";

        private long m_userId;

        public event WorkComplete WorkCompleteEventHandler;

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            m_userId = _userId;
        }

        public Response OnMessage(string _message)
        {
            WorkCompleteEventHandler?.Invoke(m_userId);

            return new Response("/register - чтобы зарегистрироваться \n" +
                "/manage - для управления аккаунтом \n" +
                "исходники бота можно найти тут https://github.com/mrGoner/VkToTelegramGrabber");
        }
    }
}
