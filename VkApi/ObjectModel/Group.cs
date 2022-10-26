using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel
{
    public class Group
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("screen_name")]
        public string ScreenName { get; set; }
        
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
        
        [JsonPropertyName("photo_50")]
        public string PhotoSmall { get; set; }
        
        [JsonPropertyName("photo_100")]
        public string PhotoMedium { get; set; }
        
        [JsonPropertyName("photo_200")]
        public string PhotoLarge { get; set; }
    }
}