using System;
namespace VkTools.ObjectModel.Attachments
{
    public class UnsupportedAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Unsupported;

        public string UnSupportedType { get; }

        public UnsupportedAttachment(string _type)
        {
            UnSupportedType = _type;
        }
    }
}
