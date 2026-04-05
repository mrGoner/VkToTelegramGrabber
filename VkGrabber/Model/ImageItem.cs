namespace VkGrabber.Model;

public readonly record struct ImageItem(string? Text, string? UrlSmall, string? UrlMedium, string? UrlLarge) : IPostItem;