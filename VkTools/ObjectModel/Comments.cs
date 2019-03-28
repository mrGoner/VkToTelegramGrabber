namespace VkTools.ObjectModel
{
    /// <summary>
    /// For more https://vk.com/dev/objects/post
    /// </summary>
    public struct Comments
    {
        public int Count { get; }
        public bool CanPost { get; }
        public bool? GroupCanPost { get; }

        public Comments(int _count, bool _canPost, bool? _groupCanPost)
        {
            Count = _count;
            CanPost = _canPost;
            GroupCanPost = _groupCanPost;
        }
    }
}