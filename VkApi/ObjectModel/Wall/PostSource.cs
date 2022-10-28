using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel.Wall
{
    /// <summary>
    /// For more see https://vk.com/dev/objects/post_source
    /// </summary>
    public class PostSource
    {
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PostSourceType Type { get; set; }

        [JsonPropertyName("platform")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlatformType? Platfrom { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public enum PlatformType
    {
        Android,
        Iphone,
        WPhone,
        Unknown
    }
}