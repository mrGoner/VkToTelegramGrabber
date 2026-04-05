namespace VkGrabber.Model;

public readonly record struct DocumentItem(string? Title, string Url) : IPostItem;