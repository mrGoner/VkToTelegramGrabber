﻿using System;
using VkTools.ObjectModel.Attachments;

namespace VkTools.ObjectModel.Wall
{
    public class HistoryPost
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int FromId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public IAttachmentElement[] Attachments { get; set; }
        public PostSource PostSource { get; set; }
    }
}