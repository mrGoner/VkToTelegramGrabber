using System;

namespace VkTools.ObjectModel.Attachments.Photo
{
    /// <summary>
    /// Photo attachment. For more https://vk.com/dev/objects/photo
    /// </summary>
    public class PhotoAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Photo;

        public int Id { get; internal set; }
        public int AlbumId { get; internal set; }
        public int OwnerId { get; internal set; }
        public int? UserId { get; internal set; }
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        public PhotoSizeInfo[] Sizes { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}