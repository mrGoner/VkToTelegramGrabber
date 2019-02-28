using System;

namespace VkGrabber
{
    public class Group
    {
        public string Prefix { get; }
        public int GroupId { get; }
        public TimeSpan UpdatePeriod { get; }

        public Group(int _groupId, string _prefix, TimeSpan _updatePeriod)
        {
            GroupId = _groupId;
            Prefix = _prefix;
            UpdatePeriod = _updatePeriod;
        }
    }
}