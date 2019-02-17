using System;
using System.Collections.Generic;

namespace VkTools
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

    public class NewsFeed : List<NewsFeedElement> { }

    public enum NewsFeedType
    {
        Post
    }

    public abstract class NewsFeedElement
    {
        public abstract NewsFeedType Type { get; }
    }

    public abstract class AttachmentElement
    {
        public abstract AttachmentElementType Type { get; }
    }

    public enum AttachmentElementType
    {
        Photo,
        Music
    }

    public class PhotoAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Photo;
        public PhotoInfo Info { get; internal set; }
    }

    public class PhotoInfo
    {
        public int Id { get; internal set; }
        public int AlbumId { get; internal set; }
        public int OwnerId { get; internal set; }
        public int UserId { get; internal set; }
        public PhotoSizeInfo[] Sizes { get; internal set; }
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        public string AccessKey { get; internal set; }
    }

    public struct PhotoSizeInfo
    {
        public PhotoSizeType Type { get; internal set; }
        public string Url { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
    }

    /// <summary>
    /// Photo size type. For more see https://vk.com/dev/photo_sizes
    /// </summary>
    public enum PhotoSizeType
    {
        m,
        o,
        p,
        q,
        r,
        s,
        x,
        y,
        z,
        w
    }

    public class PostSource
    {
        public PostSourceType Type { get; }
    }

    public enum PostSourceType
    {
        VK
    }

    public struct Comments
    {
        public int Count { get; set; }
        public bool CanPost { get; set; }
        public bool GroupCanPost { get; set; }
    }

    public struct Likes
    {
        public int Count { get; }
        public int UserLkes { get; }
        public bool UserLike { get; }
        public bool CanPublish { get; }
    }

    public struct Reposts
    {
        public int Count { get; set; }
        public int UserReposted { get; set; }
    }

    public struct Views
    {
        public int Count { get; set; }
    }
}