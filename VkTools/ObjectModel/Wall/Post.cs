using System;
using VkTools.ObjectModel.Attachments;

namespace VkTools.ObjectModel.Wall
{
    public class Post : INewsFeedElement
    {
        public NewsFeedType Type => NewsFeedType.Post;

        public int SourceId { get; internal set; }
        public int PostId { get; internal set; }
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        public int? SignerId { get; internal set; }
        public bool MarkedAsAds { get; internal set; }
        public IAttachmentElement[] Attachments { get; internal set; }
        public PostSource PostSource { get; internal set; }
        public Comments Comments { get; internal set; }
        public Likes Likes { get; internal set; }
        public Reposts Reposts { get; internal set; }
        public Views Views { get; internal set; }
        public bool IsFavorite { get; internal set; }
        public HistoryPost[] CopyHistory { get; internal set; }
    }
}