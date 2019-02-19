namespace VkTools.ObjectModel
{
    public struct Reposts
    {
        public int Count { get; }
        public bool UserReposted { get; }

        public Reposts(int _count, bool _userReposted)
        {
            Count = _count;
            UserReposted = _userReposted;
        }
    }
}