using System;

namespace TelegramBot.Helpers
{
    internal class PostLikeHelper
    {
        public static string SerializeInfo(LikeInfo _likeInfo)
        {
            return $"{_likeInfo.OwnerId};{_likeInfo.ItemId};{(_likeInfo.IsLiked ? 1 : 0)}";
        }

        public static bool TryParseLikeInfo(string _data, out LikeInfo _likeInfo)
        {
            _likeInfo = null;

            try
            {
                var values = _data.Split(';', StringSplitOptions.RemoveEmptyEntries);

                _likeInfo = new LikeInfo
                {
                    OwnerId = int.Parse(values[0]),
                    ItemId = int.Parse(values[1]),
                    IsLiked = int.Parse(values[2]) == 1
                };

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}