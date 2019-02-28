namespace VkGrabber.Model
{
    public struct AudioItem : IPostItem
    {
        public string Name { get; }
        public string Artist { get; }
        public string Url { get; }

        public AudioItem(string _name, string _artist, string _url)
        {
            Name = _name;
            Artist = _artist;
            Url = _url;
        }
    }
}