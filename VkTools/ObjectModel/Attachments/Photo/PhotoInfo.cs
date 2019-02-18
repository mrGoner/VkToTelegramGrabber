using System;

namespace VkTools.ObjectModel.Attachments.Photo
{
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
}