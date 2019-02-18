﻿using System;
using VkTools.ObjectModel.Attachments;

namespace VkTools.ObjectModel.Wall
{
    public class Post : NewsFeedElement
    {
        public override NewsFeedType Type => NewsFeedType.Post;

        public int SourceId { get; internal set; }
        public int PostId { get; internal set; }
        public string PostType { get; internal set; } //todo type!
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        public int? SignerId { get; internal set; }
        public bool MarkedAsAds { get; internal set; }
        public AttachmentElement[] Attachments { get; internal set; }
        public PostSource PostSource { get; internal set; }
        public Comments Comments { get; internal set; }
        public Likes Likes { get; internal set; }
        public Reposts Reposts { get; internal set; }
        public bool IsFavorite { get; internal set; }
    }
}