using System;
using System.Text.Json.Serialization;
using VkTools.Converters;

namespace VkTools.ObjectModel.Attachments.Doc
{
    /// <summary>
    /// Document attachment. For more https://vk.com/dev/objects/doc
    /// </summary>
    public class DocumentAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Doc;

        [JsonPropertyName("id")]
        public int Id { get; internal set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; internal set; }
        
        [JsonPropertyName("title")]
        public string Title { get; internal set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; internal set; }
        
        [JsonPropertyName("url")]
        public string Url { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}