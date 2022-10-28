using System.Text.Json.Serialization;

namespace VkApi.ObjectModel.Attachments.Audio
{
    /// <summary>
    /// Audio attachment. For more https://dev.vk.com/reference/objects/audio
    /// </summary>
    public class AudioAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Audio;

        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; set; }
        
        [JsonPropertyName("artist")]
        public string Artist { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
    }
}