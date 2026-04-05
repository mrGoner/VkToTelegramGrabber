using System.Threading;
using System.Threading.Tasks;
using VkGrabber;
using VkApi;

namespace TelegramBot.UserHelpers;

public delegate void WorkComplete(long key);

public interface IUserHelper
{
    event WorkComplete? WorkCompleteEventHandler;
    ValueTask<Response?> ProcessMessageAsync(string message, CancellationToken cancellationToken);
    void Init(long userId, Vk vkApi, UserManager userManager);
}