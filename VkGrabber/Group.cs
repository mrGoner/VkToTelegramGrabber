using System;

namespace VkGrabber;

public class Group
{
    public string Prefix { get; } = "g";
    public int GroupId { get; }
    public TimeSpan UpdatePeriod { get; }
    public string Name { get; set; }

    public Group(int groupId, TimeSpan updatePeriod, string name)
    {
        GroupId = groupId;
        UpdatePeriod = updatePeriod;
        Name = name;
    }
}