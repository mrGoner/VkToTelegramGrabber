using System.Text.Json.Serialization;

namespace VkTools.ObjectModel.Attachments.Audio
{
    /// <summary>
    /// Audio attachment. For more https://dev.vk.com/reference/objects/audio
    /// </summary>
    public class AudioAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Audio;

        [JsonPropertyName("id")]
        public int Id { get; internal set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; internal set; }
        
        [JsonPropertyName("artist")]
        public string Artist { get; internal set; }
        
        [JsonPropertyName("title")]
        public string Title { get; internal set; }
        
        [JsonPropertyName("url")]
        public string Url { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}