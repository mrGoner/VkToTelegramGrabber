using TelegramBot.UserHelpers;

namespace TelegramBot
{
    public interface IUserHelperSelector
    {
        bool TryGetCompatibleHelper(string _command, out IUserHelper _helper);

        IDefaultHelper DefaultHelper { get; }
    }
}