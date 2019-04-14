namespace TelegramBot
{
    public interface IUserHelperSelector
    {
        bool TryGetCompatibleHelper(string _command, out IUserHelper _helper);

        IUserHelper DefaultHelper { get; }
    }
}