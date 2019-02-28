namespace VkGrabber.Model
{
    public struct VideoItem : IPostItem
    {
        public string Url { get; }
        public string Title { get; }

        public VideoItem(string _title, string _url)
        {
            Url = _url;
            Title = _title;
        }
    }
}