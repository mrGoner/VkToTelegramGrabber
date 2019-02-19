namespace VkTools.ObjectModel.Attachments.Link
{
    /// <summary>
    /// Link attachment. For more
    /// </summary>
    public class LinkAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Link;
        public string Url { get; internal set; }
        public string Title { get; internal set; }
        public string Description { get; internal set; }
    }
}