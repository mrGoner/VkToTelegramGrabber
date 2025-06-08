using System;
using System.Collections.Generic;
using System.Linq;
using VkGrabber.Model;

namespace TelegramBot.Queue;

internal class QueuedMessageInfo
{
    public DateTime LastSendTime { get; set; }
    public bool Used { get; set; }

    public Queue<Post> Posts { get; } = new();

    public QueuedMessageInfo(Post _post)
    {
        if (_post is null)
            throw new ArgumentNullException(nameof(_post));

        Posts.Enqueue(_post);
    }

    public QueuedMessageInfo(IEnumerable<Post> _posts)
    {
        if (_posts is null)
            throw new ArgumentNullException(nameof(_posts));

        foreach (var post in _posts.ToList()) 
            Posts.Enqueue(post);
    }
}