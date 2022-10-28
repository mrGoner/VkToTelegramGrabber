using System;
using System.Text.Json;
using VkApi.ObjectModel.Wall;

namespace VkApi.Serializers
{
    public class NewsFeedDeserializer
    {
        public NewsFeed Deserialize(string _data)
        {
            try
            {
                var newsFeed = new NewsFeed();
                using var document = JsonDocument.Parse(_data);
                var items = document.RootElement.GetProperty("response").GetProperty("items");

                var deserializedPosts = items.Deserialize<Post[]>();
                newsFeed.AddRange(deserializedPosts ?? Array.Empty<Post>());

                return newsFeed;
            }
            catch (Exception ex)
            {
                throw new DeserializerException("Failed to deserialize newsfeed", ex);
            }
        }
    }
}