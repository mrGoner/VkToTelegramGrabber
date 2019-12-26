using VkGrabber;
using VkApi;

namespace TelegramBot.UserHelpers
{
    public delegate void WorkComplete(long _key);

    public interface IUserHelper
    {
        string Command { get; }
        event WorkComplete WorkCompleteEventHandler;
        Response OnMessage(string _message);
        void Init(long _userId, Vk _vkApi, UserManager _userManager);
    }
}
