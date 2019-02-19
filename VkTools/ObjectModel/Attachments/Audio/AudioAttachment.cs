namespace VkTools.ObjectModel.Attachments.Audio
{
    /// <summary>
    /// Audio attachment. For more
    /// </summary>
    public class AudioAttachment : AttachmentElement
    {
        public override AttachmentElementType Type => AttachmentElementType.Audio;

        public int Id { get; internal set; }
        public int OwnerId { get; internal set; }
        public string Artist { get; internal set; }
        public string Title { get; internal set; }
        public string Url { get; internal set; }
    }
}