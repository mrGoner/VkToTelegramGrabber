namespace TelegramBot
{
    public partial class Bot
    {
        public interface IUserHelperSelector
        {
            bool TryGetCompatibleHelper(string _command, out IUserHelper _helper);
        }
    }
}
