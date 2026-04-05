using System;
using System.Text.Json;
using VkApi.ObjectModel;

namespace VkApi.Serializers;

public class GroupsDeserializer
{
    public Groups Deserialize(string data)
    {
        try
        {
            using var document = JsonDocument.Parse(data);

            var groups = document.RootElement.GetProperty("response").GetProperty("items").Deserialize<Group[]>() ??
                         throw new InvalidOperationException($"Failed to deserialize groups from data {data}");

            return new Groups(groups);
        }
        catch (Exception ex)
        {
            throw new DeserializerException("Failed to deserialize groups", data, ex);
        }
    }
}