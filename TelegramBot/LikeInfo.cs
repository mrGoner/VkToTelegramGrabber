using System;

namespace TelegramBot
{
    [Serializable]
    internal class LikeInfo
    {
        public int OwnerId { get; set; }
        public int ItemId { get; set; }
        public bool IsLiked { get; set; }
    }
}