namespace VkTools.ObjectModel
{
    /// <summary>
    /// For more https://vk.com/dev/objects/post
    /// </summary>
    public struct Comments
    {
        public int Count { get; set; }
        public bool CanPost { get; set; }
        public bool GroupCanPost { get; set; }
    }
}