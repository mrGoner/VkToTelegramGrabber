using System;
using System.Text.Json.Serialization;
using VkTools.Converters;
using VkTools.ObjectModel.Attachments;

namespace VkTools.ObjectModel.Wall
{
    public class Post : INewsFeedElement
    {
        public NewsFeedType Type => NewsFeedType.Post;

        public int SourceId { get; internal set; }
        
        [JsonPropertyName("id")]
        public int Id { get; internal set; }
        
        [JsonPropertyName("text")]
        public string Text { get; internal set; }
        
        [JsonPropertyName("date")]
        [JsonConverter(typeof(EpochTimeConverter))]
        public DateTime Date { get; internal set; }
        
        [JsonPropertyName("signer_id")]
        public int? SignerId { get; internal set; }
        
        [JsonPropertyName("marked_as_ads")]
        [JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool MarkedAsAds { get; internal set; }
        
        [JsonPropertyName("attachments")]
        [JsonConverter(typeof(AttachmentsJsonConverter))]
        public IAttachmentElement[] Attachments { get; internal set; }
        public PostSource PostSource { get; internal set; }
        public Comments Comments { get; internal set; }
        public Likes Likes { get; internal set; }
        public Reposts Reposts { get; internal set; }
        public Views Views { get; internal set; }
        
        [JsonPropertyName("is_favorite")]
        public bool IsFavorite { get; internal set; }
        public HistoryPost[] CopyHistory { get; internal set; } = new HistoryPost[0];
    }
}