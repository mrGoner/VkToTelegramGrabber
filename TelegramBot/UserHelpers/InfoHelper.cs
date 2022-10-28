using System.Threading;
using System.Threading.Tasks;
using VkApi;
using VkGrabber;
using TelegramBot.Helpers;

namespace TelegramBot.UserHelpers
{
    public class InfoHelper : IUserHelper
    {
        public string Command => "/help";
        public event WorkComplete WorkCompleteEventHandler;
        private long m_userId;

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            m_userId = _userId;
        }

        public ValueTask<Response> ProcessMessageAsync(string _message, CancellationToken _cancellationToken)
        {
            WorkCompleteEventHandler?.Invoke(m_userId);
            return new ValueTask<Response>(new Response("/register - чтобы зарегистрироваться \n" +
                                                        "/manage - для управления аккаунтом \n" +
                                                        "исходники бота можно найти тут https://github.com/mrGoner/VkToTelegramGrabber", KeyBoardBuilder.EmptyKeyboard));
        }
    }
}
