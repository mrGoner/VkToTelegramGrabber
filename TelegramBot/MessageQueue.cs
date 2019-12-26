using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VkGrabber.Model;

namespace TelegramBot
{
    internal class MessageQueue
    {
        private readonly Dictionary<long, QueuedMessageInfo> m_messages;
        private readonly TimeSpan m_timeoutBetweenMessages;
        private readonly int m_threadCount;
        private readonly Action<long,Post> m_sendAction;
        private readonly object m_syncObject = new object();
        private readonly AutoResetEvent m_autoResetEvent;

        public MessageQueue(TimeSpan _timeoutBetweenMessages, int _threadCount, Action<long,Post> _sendAction)
        {
            m_sendAction = _sendAction ?? throw new ArgumentNullException(nameof(_sendAction));

            m_messages = new Dictionary<long, QueuedMessageInfo>();
            m_timeoutBetweenMessages = _timeoutBetweenMessages;
            m_threadCount = _threadCount;

            m_autoResetEvent = new AutoResetEvent(false);

            for(int i = 0; i < _threadCount; i++)
            {
                Task.Factory.StartNew(ProcessMessageQueue, TaskCreationOptions.LongRunning);
            }
        }

        public async Task ProcessMessageQueue()
        {
            while (true)
            {
                Monitor.Enter(m_syncObject);

                if (m_messages.Any())
                {
                    var keyValue = m_messages.FirstOrDefault(_x =>
                    ((DateTime.Now - _x.Value.LastSendTime) > m_timeoutBetweenMessages) && _x.Value.Posts.Any());

                    var messageInfo = keyValue.Value;

                    if (messageInfo != null)
                    {
                        if (messageInfo.Posts.TryDequeue(out var postToSend))
                        {
                            messageInfo.LastSendTime = DateTime.Now;

                            try
                            {
                                m_sendAction(keyValue.Key, postToSend);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Failed to send message {ex}");
                                //todo enqueue postToSend?
                            }
                        }

                        if (messageInfo.Posts.Count == 0)
                            m_messages.Remove(keyValue.Key);

                        Monitor.Exit(m_syncObject);
                    }
                    else
                    {
                        Monitor.Exit(m_syncObject);

                        await Task.Delay(1000);
                    }
                }
                else
                {
                    Monitor.Exit(m_syncObject);

                    m_autoResetEvent.WaitOne();
                }
            }
        }

        public void AddMessage(Message _message)
        {
            if (_message is null)
                throw new ArgumentNullException(nameof(_message));

            lock (m_syncObject)
            {
                if(m_messages.TryGetValue(_message.UserId, out var messagesInfo))
                {
                    messagesInfo.Posts.Enqueue(_message.PostToSend);
                }
                else
                {
                    m_messages.Add(_message.UserId, new QueuedMessageInfo(_message.PostToSend));
                }
            }

            m_autoResetEvent.Set();
        }
    }

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

    internal class QueuedMessageInfo
    {
        public DateTime LastSendTime { get; set; }

        public Queue<Post> Posts { get; } = new Queue<Post>();

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
            {
                Posts.Enqueue(post);
            }
        }
    }
}
