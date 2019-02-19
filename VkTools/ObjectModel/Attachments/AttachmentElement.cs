namespace VkTools.ObjectModel.Attachments
{
    public abstract class AttachmentElement
    {
        public abstract AttachmentElementType Type { get; }
        public string AccessKey { get; internal set; }
    }
}