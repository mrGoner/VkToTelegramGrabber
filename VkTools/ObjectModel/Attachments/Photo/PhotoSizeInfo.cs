namespace VkTools.ObjectModel.Attachments.Photo
{
    public struct PhotoSizeInfo
    {
        public PhotoSizeType Type { get; }
        public string Url { get; }
        public int Width { get; }
        public int Height { get; }

        public PhotoSizeInfo(PhotoSizeType _type, string _url, int _width, int _heigh)
        {
            Type = _type;
            Url = _url;
            Width = _width;
            Height = _heigh;
        }
    }
}