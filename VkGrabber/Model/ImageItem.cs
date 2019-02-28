namespace VkGrabber.Model
{
    public struct ImageItem : IPostItem
    {
        public string Text { get; }
        public string UrlSmall { get; }
        public string UrlMedium { get; }
        public string UrlLarge { get; }

        public ImageItem(string _text, string _smallUrl, string _mediumUrl, string _largeUrl)
        {
            Text = _text;
            UrlSmall = _smallUrl;
            UrlMedium = _mediumUrl;
            UrlLarge = _largeUrl;
        }
    }
}