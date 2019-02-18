namespace VkTools.ObjectModel.Attachments.Photo
{
    public class PhotoAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Photo;
        public PhotoInfo Info { get; internal set; }
    }
}