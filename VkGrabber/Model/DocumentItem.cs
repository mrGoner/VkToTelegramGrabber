namespace VkGrabber.Model
{
    public struct DocumentItem : IPostItem
    {
        public string Url { get; }
        public string Title { get; }

        public DocumentItem(string _title, string _url)
        {
            Url = _url;
            Title = _title;
        }
    }
}