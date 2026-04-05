using System;
using System.Text.Json;

namespace VkApi.Serializers;

public class LikesDeserializer
{
    public int ParseLikesCount(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("data can not be null or empty", nameof(data));

        try
        {
            return JsonDocument.Parse(data).RootElement.GetProperty("response").GetProperty("likes").GetInt32();
        }
        catch (Exception ex)
        {
            throw new DeserializerException("Failed to parse likes count", data, ex);
        }
    }
}