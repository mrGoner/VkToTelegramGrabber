using System.Text.Json.Serialization;

namespace VkTools.ObjectModel.Attachments.Photo
{
    public struct PhotoSizeInfo
    {
        [JsonPropertyName("type")]
        public PhotoSizeType Type { get; set;}
        
        [JsonPropertyName("url")]
        public string Url { get; set;}
        
        [JsonPropertyName("width")]
        public int Width { get; set; }
        
        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}