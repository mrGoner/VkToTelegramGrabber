using System;

namespace VkGrabber
{
    public class Group
    {
        public string Prefix { get; } = "g";
        public int GroupId { get; }
        public TimeSpan UpdatePeriod { get; }
        public string Name { get; set; }

        public Group(int _groupId, TimeSpan _updatePeriod, string _name)
        {
            GroupId = _groupId;
            UpdatePeriod = _updatePeriod;
            Name = _name;
        }
    }
}