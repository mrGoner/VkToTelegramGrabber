using System.Threading;
using System.Threading.Tasks;
using VkApi;
using VkGrabber;
using TelegramBot.Helpers;

namespace TelegramBot.UserHelpers;

public class InfoHelper : IUserHelper
{
    public static string Command => "/help";
    public event WorkComplete? WorkCompleteEventHandler;
    private long _userId;

    public void Init(long userId, Vk vkApi, UserManager userManager)
    {
        _userId = userId;
    }

    public ValueTask<Response?> ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        WorkCompleteEventHandler?.Invoke(_userId);

        return new ValueTask<Response?>(new Response("/register - чтобы зарегистрироваться \n" +
                                                     "/manage - для управления аккаунтом \n" +
                                                     "исходники бота можно найти тут https://github.com/mrGoner/VkToTelegramGrabber",
            KeyBoardBuilder.EmptyKeyboard));
    }
}