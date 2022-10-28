using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel
{
    /// <summary>
    /// For more https://vk.com/dev/objects/post
    /// </summary>
    public struct Comments
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        
        [JsonPropertyName("can_post")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool CanPost { get; set; }
        
        [JsonPropertyName("groupts_can_post")]
        public bool? GroupCanPost { get; set; }
        
    }
}