using System;
using System.Text.Json;
using VkApi.ObjectModel.Wall;

namespace VkApi.Serializers
{
    public class NewsFeedItemsDeserializer
    {
        public (INewsFeedElement[] Items, string NextToken) Deserialize(string _data)
        {
            try
            {
                using var document = JsonDocument.Parse(_data);
                var response = document.RootElement.GetProperty("response");

                var deserializedPosts = response.GetProperty("items").Deserialize<Post[]>();

                string nextToken = null;

                if(response.TryGetProperty("next_from", out var nextFromProp))
                    nextToken = nextFromProp.GetString();

                return (deserializedPosts ?? Array.Empty<Post>(), nextToken);
            }
            catch (Exception ex)
            {
                throw new DeserializerException("Failed to deserialize newsfeed", _data, ex);
            }
        }
    }
}