namespace VkTools.ObjectModel.Attachments.Audio
{
    /// <summary>
    /// Audio attachment. For more
    /// </summary>
    public class AudioAttachment : IAttachmentElement
    {
        public AttachmentElementType Type => AttachmentElementType.Audio;

        public int Id { get; internal set; }
        public int OwnerId { get; internal set; }
        public string Artist { get; internal set; }
        public string Title { get; internal set; }
        public string Url { get; internal set; }

        public string AccessKey { get; internal set; }
    }
}