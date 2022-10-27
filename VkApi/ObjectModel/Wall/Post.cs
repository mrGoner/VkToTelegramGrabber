using System;
using System.Text.Json.Serialization;
using VkApi.Converters;
using VkApi.ObjectModel.Attachments;

namespace VkApi.ObjectModel.Wall
{
    public class Post : INewsFeedElement
    {
        public NewsFeedType Type => NewsFeedType.Post;

        [JsonPropertyName("source_id")]
        public int SourceId { get; set; }
        
        [JsonPropertyName("post_id")]
        public int Id { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeJsonConverter))]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("signer_id")]
        public int? SignerId { get; set; }
        
        [JsonPropertyName("marked_as_ads")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool MarkedAsAds { get; set; }
        
        [JsonPropertyName("attachments")]
        [JsonConverter(typeof(AttachmentsJsonConverter))]
        public IAttachmentElement[] Attachments { get; set; } = Array.Empty<IAttachmentElement>();
        
        [JsonPropertyName("post_source")]
        public PostSource PostSource { get; set; }
        
        [JsonPropertyName("comments")]
        public Comments Comments { get; set; }
       
        [JsonPropertyName("likes")]
        public Likes Likes { get; set; }
        
        [JsonPropertyName("reposts")]
        public Reposts Reposts { get; set; }
        public Views Views { get; set; }
        
        [JsonPropertyName("is_favorite")]
        public bool IsFavorite { get; set; }
        
        [JsonPropertyName("copy_history")]
        public Post[] CopyHistory { get; set; } = Array.Empty<Post>();
    }
}