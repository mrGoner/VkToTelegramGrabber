using System;
using System.Text.Json.Serialization;
using VkTools.Converters;

namespace VkTools.ObjectModel.Attachments.Photo
{
    /// <summary>
    /// Photo attachment. For more https://vk.com/dev/objects/photo
    /// </summary>
    public class PhotoAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Photo;

        [JsonPropertyName("id")]
        public int Id { get; internal set; }
        
        [JsonPropertyName("album_id")]
        public int AlbumId { get; internal set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; internal set; }
        
        [JsonPropertyName("user_id")]
        public int? UserId { get; internal set; }
        
        [JsonPropertyName("text")]
        public string Text { get; internal set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; internal set; }
        
        [JsonPropertyName("sizes")]
        public PhotoSizeInfo[] Sizes { get; internal set; } = new PhotoSizeInfo[0];

        public string AccessKey { get; internal set; }
    }
}