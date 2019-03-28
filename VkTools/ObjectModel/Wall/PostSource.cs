namespace VkTools.ObjectModel.Wall
{
    /// <summary>
    /// For more see https://vk.com/dev/objects/post_source
    /// </summary>
    public class PostSource
    {
        public PostSourceType Type { get; internal set; }
        public PlatformType? Platfrom { get; internal set; }
        public string Data { get; internal set; }
        public string Url { get; internal set; }
    }

    public enum PlatformType
    {
        Android,
        Iphone,
        WPhone,
        Unknown
    }
}