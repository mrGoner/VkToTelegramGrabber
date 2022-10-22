using System.Text.Json.Serialization;

namespace VkTools.ObjectModel.Attachments.Link
{
    /// <summary>
    /// Link attachment. For more https://dev.vk.com/reference/objects/link
    /// </summary>
    public class LinkAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Link;
        
        [JsonPropertyName("url")]
        public string Url { get; internal set; }
        
        [JsonPropertyName("title")]
        public string Title { get; internal set; }
        
        [JsonPropertyName("description")]
        public string Description { get; internal set; }

        [JsonPropertyName("caption")]
        public string Caption { get; internal set; }
        public string AccessKey { get; internal set; }
    }
}