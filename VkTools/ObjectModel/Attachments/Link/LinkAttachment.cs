namespace VkTools.ObjectModel.Attachments.Link
{
    /// <summary>
    /// Link attachment. For more
    /// </summary>
    public class LinkAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Link;
        public string Url { get; internal set; }
        public string Title { get; internal set; }
        public string Description { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}