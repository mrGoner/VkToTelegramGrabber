namespace TelegramBot.UserHelpers
{

    public class DefaultHelper : IDefaultHelper
    {
        public Response GetDefaultResponce()
        {
            return new Response("Неопознанная команда! \n /register для регистрации \n /manage для управления группами и аккаунтом");
        }
    }
}