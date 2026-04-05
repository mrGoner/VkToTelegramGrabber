using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel;

public class Group
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("name")] 
    public string Name { get; set; } = null!;

    [JsonPropertyName("screen_name")] 
    public string? ScreenName { get; set; }

    [JsonPropertyName("is_closed")] 
    public int IsClosed { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GroupType Type { get; set; }

    [JsonPropertyName("is_admin")]
    [JsonConverter(typeof(IntToBoolJsonConverter))]
    public bool IsAdmin { get; set; }

    [JsonPropertyName("is_member")]
    [JsonConverter(typeof(IntToBoolJsonConverter))]
    public bool IsMember { get; set; }

    [JsonPropertyName("is_advertiser")]
    [JsonConverter(typeof(IntToBoolJsonConverter))]
    public bool IsAdvertiser { get; set; }
}