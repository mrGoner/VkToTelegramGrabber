using VkApi;
using VkGrabber;

namespace TelegramBot
{

    public class UserInfoHelper : IUserHelper
    {
        public string Command => throw new System.NotImplementedException("It's default helper!");

        public event WorkComplete WorkCompleteEventHandler;

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            throw new System.NotImplementedException("It's default helper!");
        }

        public Response OnCallBackUpdate()
        {
            throw new System.NotImplementedException();
        }

        public Response OnMessage(string _message)
        {
            return new Response("Неопознанная команда! \n /register для регистрации \n /manage для управления группами и аккаунтом");
        }
    }
}