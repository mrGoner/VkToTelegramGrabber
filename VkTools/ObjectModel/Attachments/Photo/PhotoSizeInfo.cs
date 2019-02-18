namespace VkTools.ObjectModel.Attachments.Photo
{
    public struct PhotoSizeInfo
    {
        public PhotoSizeType Type { get; internal set; }
        public string Url { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
    }
}