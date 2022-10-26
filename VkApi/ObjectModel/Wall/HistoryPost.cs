using System;
using VkApi.ObjectModel.Attachments;

namespace VkApi.ObjectModel.Wall
{
    public class HistoryPost
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int FromId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public IAttachmentElement[] Attachments { get; set; } = new IAttachmentElement[0];
        public PostSource PostSource { get; set; }
    }
}