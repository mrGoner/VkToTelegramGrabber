namespace VkGrabber.Model;

public struct NoteItem : IPostItem
{
    public string Title { get; }
    public string Text { get; }

    public NoteItem(string _title, string _text)
    {
        Title = _title;
        Text = _text;
    }
}