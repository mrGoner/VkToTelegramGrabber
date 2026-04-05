using System;

namespace VkGrabber.Model;

public record Post(int PostId, int GroupId, DateTime PublishTime, string GroupName, string? Text, IPostItem[] Items);