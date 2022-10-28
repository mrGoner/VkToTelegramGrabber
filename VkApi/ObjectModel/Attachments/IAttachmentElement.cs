namespace VkApi.ObjectModel.Attachments
{
    public interface IAttachmentElement
    {
        AttachmentElementType Type { get; }
        string AccessKey { get; }
    }
}