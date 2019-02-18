namespace VkTools.ObjectModel
{
    /// <summary>
    /// For more https://vk.com/dev/objects/post
    /// </summary>
    public struct Likes
    {
        public int Count { get; }
        public bool UserLkes { get; }
        public bool CanLike { get; }
        public bool CanPublish { get; }

        public Likes(int _count, bool _userLikes, bool _canLike, bool _canPublish)
        {
            Count = _count;
            UserLkes = _userLikes;
            CanLike = _canLike;
            CanPublish = _canPublish;
        }
    }
}