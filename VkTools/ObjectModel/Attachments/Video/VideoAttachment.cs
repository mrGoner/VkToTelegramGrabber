using System;
using System.Text.Json.Serialization;
using VkTools.Converters;

namespace VkTools.ObjectModel.Attachments.Video
{
    /// <summary>
    /// Video attachment. https://vk.com/dev/objects/video
    /// </summary>
    public class VideoAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Video;

        [JsonPropertyName("id")]
        public int Id { get; internal set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; internal set; }
        
        [JsonPropertyName("title")]
        public string Title { get; internal set; }
        
        [JsonPropertyName("description")]
        public string Description { get; internal set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; internal set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; internal set; }
        
        [JsonPropertyName("views")]
        public int Views { get; internal set; }
        
        [JsonPropertyName("comments")]
        public int? CommentsCount { get; internal set; }
        
        [JsonPropertyName("player")]
        public string PlayerUrl { get; internal set; }
        public string AccessKey { get; internal set; }
        
        [JsonPropertyName("image")]
        public Image[] Images { get; internal set; }
        
        [JsonPropertyName("first_frame")]
        public Image[] FirstFrames { get; internal set; }
        
        [JsonPropertyName("content_restricted")]
        [JsonConverter(typeof(FieldToBoolJsonConverter))]
        public bool? IsContentRestricted { get; set; }
        
        [JsonPropertyName("content_restricted_message")]
        public string ContentRestrictedMessage { get; set; }
    }
}