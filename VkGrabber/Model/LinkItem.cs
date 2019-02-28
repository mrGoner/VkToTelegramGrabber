namespace VkGrabber.Model
{
    public struct LinkItem : IPostItem
    {
        public string Url { get; }

        public LinkItem(string _url)
        {
            Url = _url;
        }
    }
}