using System.Threading;
using System.Threading.Tasks;
using VkGrabber;
using VkApi;

namespace TelegramBot.UserHelpers
{
    public delegate void WorkComplete(long _key);

    public interface IUserHelper
    {
        string Command { get; }
        event WorkComplete WorkCompleteEventHandler;
        ValueTask<Response> ProcessMessageAsync(string _message, CancellationToken _cancellationToken);
        void Init(long _userId, Vk _vkApi, UserManager _userManager);
    }
}
