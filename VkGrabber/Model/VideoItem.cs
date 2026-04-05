namespace VkGrabber.Model;

public readonly record struct VideoItem(string? Title, string Url) : IPostItem;