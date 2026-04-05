using System;
using VkGrabber.Model;

namespace TelegramBot.Queue;

internal class Message(long userId, Post postToSend)
{
    public long UserId { get; } = userId;
    public Post PostToSend { get; } = postToSend ?? throw new ArgumentNullException(nameof(postToSend));
}