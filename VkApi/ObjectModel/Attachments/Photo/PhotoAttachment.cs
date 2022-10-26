using System;
using System.Text.Json.Serialization;
using VkApi.Converters;

namespace VkApi.ObjectModel.Attachments.Photo
{
    /// <summary>
    /// Photo attachment. For more https://vk.com/dev/objects/photo
    /// </summary>
    public class PhotoAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Photo;

        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("album_id")]
        public int AlbumId { get; set; }
        
        [JsonPropertyName("owner_id")]
        public int OwnerId { get; set; }
        
        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("sizes")]
        public PhotoSizeInfo[] Sizes { get; set; } = new PhotoSizeInfo[0];

        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
    }
}