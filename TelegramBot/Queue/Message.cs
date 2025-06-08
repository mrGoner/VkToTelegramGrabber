using System;
using VkGrabber.Model;

namespace TelegramBot.Queue;

internal class Message
{
    public long UserId { get; }
    public Post PostToSend { get; }

    public Message(long _userId, Post _postToSend)
    {
        UserId = _userId;
        PostToSend = _postToSend ?? throw new ArgumentNullException(nameof(_postToSend));
    }
}