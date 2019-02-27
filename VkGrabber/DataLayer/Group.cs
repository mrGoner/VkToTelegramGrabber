using System;

namespace VkGrabber.DataLayer
{
    public class DbGroup
    {
        public int Id { get; set; }
        public string GroupPrefix { get; set; }
        public int GroupId { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int LastUpdatedPostId { get; set; }
        public TimeSpan UpdatePeriod { get; set; }
        public User User { get; set; }
        public bool IsUpdating { get; set; }
    }
}