using System;

namespace VkTools.ObjectModel.Attachments.Doc
{
    /// <summary>
    /// Document attachment. For more https://vk.com/dev/objects/doc
    /// </summary>
    public class DocumentAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Doc;

        public int Id { get; internal set; }
        public int OwnerId { get; internal set; }
        public string Title { get; internal set; }
        public DateTime Date { get; internal set; }
        public string Url { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}