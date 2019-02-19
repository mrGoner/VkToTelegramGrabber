using System;

namespace VkTools.ObjectModel.Attachments.Video
{
    /// <summary>
    /// Video attachment. https://vk.com/dev/objects/video
    /// </summary>
    public class VideoAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Video;

        public int Id { get; internal set; }
        public int OwnerId { get; internal set; }
        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public int Duration { get; internal set; }
        public DateTime Date { get; internal set; }
        public int Views { get; internal set; }
        public int CommentsCount { get; internal set; }
        public string PlayerUrl { get; internal set; }
        public bool IsFavorite { get; internal set; }
    }
}