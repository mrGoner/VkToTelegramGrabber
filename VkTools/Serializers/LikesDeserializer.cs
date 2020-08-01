using System;
using Newtonsoft.Json.Linq;

namespace VkTools.Serializers
{
    public class LikesDeserializer
    {
        #region consts

        public const string PResponse = "response";
        public const string PLikes = "likes";

        #endregion

        public int ParseLikesCount(string _data)
        {
            if (string.IsNullOrWhiteSpace(_data))
                throw new ArgumentException("data can not be null or empty", nameof(_data));

            try
            {
                var jLikes = JObject.Parse(_data);

                var count = jLikes[PResponse][PLikes].Value<int>();

                return count;
            }
            catch
            {
                throw new DeserializerException("Failed to parse likes count");
            }
        } 
    }
}
