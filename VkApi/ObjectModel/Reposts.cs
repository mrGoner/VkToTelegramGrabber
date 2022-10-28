using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel
{
    public struct Reposts
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        
        [JsonPropertyName("user_reposted")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool UserReposted { get; set; }
    }
}