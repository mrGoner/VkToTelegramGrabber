using System;
using System.Text.Json;
using VkApi.ObjectModel.Wall;

namespace VkApi.Serializers;

public class NewsFeedItemsDeserializer
{
    public (INewsFeedElement[] Items, string? NextToken) Deserialize(string data)
    {
        try
        {
            using var document = JsonDocument.Parse(data);
            var response = document.RootElement.GetProperty("response");

            var deserializedPosts = response.GetProperty("items").Deserialize<Post[]>() ??
                                    throw new InvalidOperationException($"Failed to deserialize newsfeed items from data {data}");

            string? nextToken = null;

            if (response.TryGetProperty("next_from", out var nextFromProp))
                nextToken = nextFromProp.GetString();

            return (deserializedPosts, nextToken);
        }
        catch (Exception ex)
        {
            throw new DeserializerException("Failed to deserialize newsfeed", data, ex);
        }
    }
}