using System.Text.Json.Serialization;

namespace VkTools.ObjectModel.Attachments.Video
{
    public class Image
    {
        [JsonPropertyName("height")]
        public int Height { get; internal set; }
        
        [JsonPropertyName("url")]
        public string Url { get; internal set; }
        
        [JsonPropertyName("width")]
        public int Width { get; internal set; }
    }
}