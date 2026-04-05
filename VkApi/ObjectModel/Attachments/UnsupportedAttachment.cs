namespace VkApi.ObjectModel.Attachments;

public class UnsupportedAttachment(string? type) : IAttachmentElement
{
    public AttachmentElementType Type => AttachmentElementType.Unsupported;

    public string? UnSupportedType { get; } = type;

    public string? AccessKey => null;
}