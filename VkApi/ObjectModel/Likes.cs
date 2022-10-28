using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel
{
    /// <summary>
    /// For more https://vk.com/dev/objects/post
    /// </summary>
    public struct Likes
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        
        [JsonPropertyName("user_likes")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool UserLikes { get; set; }
        
        [JsonPropertyName("can_like")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool CanLike { get; set; }
        
        [JsonPropertyName("can_publish")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool CanPublish { get; set; }
    }
}