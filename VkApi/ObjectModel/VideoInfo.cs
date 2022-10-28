using System.Text.Json.Serialization;

namespace VkApi.ObjectModel;

public class VideoInfo
{
    [JsonPropertyName("player")]
    public string PlayerUrl { get; set; }
}