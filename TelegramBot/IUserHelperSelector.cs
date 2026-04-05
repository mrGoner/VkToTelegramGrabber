using TelegramBot.UserHelpers;

namespace TelegramBot;

public interface IUserHelperSelector
{
    bool TryGetCompatibleHelper(string command, out IUserHelper? helper);

    IDefaultHelper DefaultHelper { get; }
}