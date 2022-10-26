using System.Text.Json.Serialization;

namespace VkApi.ObjectModel.Attachments.Link
{
    /// <summary>
    /// Link attachment. For more https://dev.vk.com/reference/objects/link
    /// </summary>
    public class LinkAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Link;
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
    }
}