namespace VkGrabber.Model;

public readonly record struct AudioItem(string? Name, string? Artist, string Url) : IPostItem;