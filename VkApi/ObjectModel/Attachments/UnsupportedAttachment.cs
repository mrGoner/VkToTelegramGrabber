namespace VkApi.ObjectModel.Attachments
{
    public class UnsupportedAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Unsupported;

        public string UnSupportedType { get; }

        public string AccessKey => null;

        public UnsupportedAttachment(string _type)
        {
            UnSupportedType = _type;
        }
    }
}
