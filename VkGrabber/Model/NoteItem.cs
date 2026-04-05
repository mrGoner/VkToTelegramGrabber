namespace VkGrabber.Model;

public readonly record struct NoteItem(string? Title, string Text) : IPostItem;