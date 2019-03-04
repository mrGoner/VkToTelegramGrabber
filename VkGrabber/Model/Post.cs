using System;

namespace VkGrabber.Model
{
    public class Post
    {
        public DateTime PublishTime { get; }
        public string GroupName { get; }
        public string Text { get; }
        public int PostId { get; set; }
        public int GroupId { get; set; }

        public IPostItem[] Items { get; }

        public Post(int _postId, int _groupId, DateTime _publishTime, string _groupName, string _text, IPostItem[] _postItems)
        {
            Items = _postItems;
            Text = _text;
            PublishTime = _publishTime;
            PostId = _postId;
            GroupId = _groupId;
            GroupName = _groupName;
        }
    }
}