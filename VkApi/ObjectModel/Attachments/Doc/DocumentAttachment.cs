using System;
using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel.Attachments.Doc
{
    /// <summary>
    /// Document attachment. For more https://vk.com/dev/objects/doc
    /// </summary>
    public class DocumentAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Doc;

        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
    }
}