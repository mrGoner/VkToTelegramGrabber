using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VkGrabber.Model;

namespace TelegramBot.Queue;

internal class MessageQueue
{
    private readonly Dictionary<long, QueuedMessageInfo> _messages;
    private readonly TimeSpan _timeoutBetweenMessages;
    private readonly Func<long, Post, Task> _sendAction;
    private readonly int _maxFailedCount;
    private readonly ILogger<MessageQueue> _logger;
    private readonly object _syncObject = new();
    private readonly AutoResetEvent _autoResetEvent;

    public MessageQueue(
        TimeSpan timeoutBetweenMessages, int threadCount, 
        Func<long, Post, Task> sendAction,
        int maxFailedCount,
        ILogger<MessageQueue> logger)
    {
        if (threadCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(threadCount));

        _sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));
        _maxFailedCount = maxFailedCount;
        _logger = logger;

        _messages = new Dictionary<long, QueuedMessageInfo>();
        _timeoutBetweenMessages = timeoutBetweenMessages;

        _autoResetEvent = new AutoResetEvent(false);

        for (var i = 0; i < threadCount; i++)
            Task.Factory.StartNew(ProcessMessageQueue, TaskCreationOptions.LongRunning);
    }

    private async Task ProcessMessageQueue()
    {
        while (true)
        {
            Monitor.Enter(_syncObject);

            if (_messages.Count > 0)
            {
                var (key, messageInfo) = _messages.Where(x => !x.Value.Used).FirstOrDefault(x =>
                    DateTime.Now - x.Value.LastSendTime > _timeoutBetweenMessages && x.Value.Posts.Count > 0);

                if (messageInfo != null)
                {
                    messageInfo.Used = true;

                    Monitor.Exit(_syncObject);

                    if (messageInfo.Posts.TryPeek(out var postToSend))
                    {
                        try
                        {
                            await _sendAction(key, postToSend);

                            messageInfo.LastSendTime = DateTime.Now;

                            messageInfo.Posts.Dequeue();
                            
                            messageInfo.FailedCount = 0;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send message");
                            
                            messageInfo.FailedCount++;
                            
                            if (messageInfo.FailedCount > _maxFailedCount)
                            {
                                messageInfo.Posts.Dequeue();
                                messageInfo.FailedCount = 0;
                            }
                            else
                            {
                                await Task.Delay(1000);
                            }
                        }
                    }

                    lock (_syncObject)
                    {
                        if (messageInfo.Posts.Count == 0)
                            _messages.Remove(key);
                        else
                            messageInfo.Used = false;
                    }
                }
                else
                {
                    Monitor.Exit(_syncObject);

                    await Task.Delay(1000);
                }
            }
            else
            {
                Monitor.Exit(_syncObject);

                _autoResetEvent.WaitOne();
            }
        }
    }

    public void AddMessage(Message message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        lock (_syncObject)
        {
            if (_messages.TryGetValue(message.UserId, out var messagesInfo))
                messagesInfo.Posts.Enqueue(message.PostToSend);
            else
                _messages.Add(message.UserId, new QueuedMessageInfo(message.PostToSend));
        }

        _autoResetEvent.Set();
    }
}