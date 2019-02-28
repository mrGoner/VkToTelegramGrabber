using System;

namespace VkGrabber.Model
{
    public class Post
    {
        public DateTime PublishTime { get; }
        public string GroupName { get; }
        public string Text { get; }

        IPostItem[] Items { get; }

        public Post(DateTime _publishTime, string _groupName, string _text, IPostItem[] _postItems)
        {
            Items = _postItems;
            Text = _text;
            PublishTime = _publishTime;
        }
    }
}