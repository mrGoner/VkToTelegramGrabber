﻿using VkGrabber;
using VkApi;

namespace TelegramBot
{
    public partial class Bot
    {
        public delegate void WorkComplete(long _key);

        public interface IUserHelper
        {
            string Command { get; }
            event WorkComplete WorkCompleteEventHandler;
            Response OnCallBackUpdate();
            Response OnMessage(string _message);
            void Init(long _userId, Vk _vkApi, UserManager _userManager);
        }
    }
}
