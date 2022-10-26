using System;
using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel.Attachments.Video
{
    /// <summary>
    /// Video attachment. https://vk.com/dev/objects/video
    /// </summary>
    public class VideoAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Video;

        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("views")]
        public int Views { get; set; }
        
        [JsonPropertyName("comments")]
        public int? CommentsCount { get; set; }
        
        [JsonPropertyName("player")]
        public string PlayerUrl { get; set; }

        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
        
        [JsonPropertyName("image")]
        public Image[] Images { get; set; }
        
        [JsonPropertyName("first_frame")]
        public Image[] FirstFrames { get; set; }
        
        [JsonPropertyName("content_restricted")]
        [JsonConverter(typeof(FieldToBoolJsonConverter))]
        public bool? IsContentRestricted { get; set; }
        
        [JsonPropertyName("content_restricted_message")]
        public string ContentRestrictedMessage { get; set; }
    }
}