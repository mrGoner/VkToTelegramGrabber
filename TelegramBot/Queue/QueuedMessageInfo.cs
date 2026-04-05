using System;
using System.Collections.Generic;
using VkGrabber.Model;

namespace TelegramBot.Queue;

internal class QueuedMessageInfo
{
    public DateTime LastSendTime { get; set; }
    public bool Used { get; set; }
    
    public int FailedCount { get; set; }

    public Queue<Post> Posts { get; } = new();

    public QueuedMessageInfo(Post post)
    {
        if (post is null)
            throw new ArgumentNullException(nameof(post));

        Posts.Enqueue(post);
    }

    public QueuedMessageInfo(IEnumerable<Post> posts)
    {
        if (posts is null)
            throw new ArgumentNullException(nameof(posts));

        foreach (var post in posts) 
            Posts.Enqueue(post);
    }
}