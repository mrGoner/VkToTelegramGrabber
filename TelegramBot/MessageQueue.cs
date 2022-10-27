using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VkGrabber.Model;

namespace TelegramBot
{
    
    //todo rewrite
    internal class MessageQueue
    {
        private readonly Dictionary<long, QueuedMessageInfo> m_messages;
        private readonly TimeSpan m_timeoutBetweenMessages;
        private readonly Func<long,Post,Task> m_sendAction;
        private readonly object m_syncObject = new object();
        private readonly AutoResetEvent m_autoResetEvent;

        public MessageQueue(TimeSpan _timeoutBetweenMessages, int _threadCount, Func<long,Post,Task> _sendAction)
        {
            if (_threadCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(_threadCount));
            
            m_sendAction = _sendAction ?? throw new ArgumentNullException(nameof(_sendAction));

            m_messages = new Dictionary<long, QueuedMessageInfo>();
            m_timeoutBetweenMessages = _timeoutBetweenMessages;

            m_autoResetEvent = new AutoResetEvent(false);

            for(int i = 0; i < _threadCount; i++)
            {
                Task.Factory.StartNew(ProcessMessageQueue, TaskCreationOptions.LongRunning);
            }
        }

        private async Task ProcessMessageQueue()
        {
            while (true)
            {
                Monitor.Enter(m_syncObject);

                if (m_messages.Any())
                {
                    var keyValue = m_messages.Where(_x => !_x.Value.Used).FirstOrDefault(_x =>
                        ((DateTime.Now - _x.Value.LastSendTime) > m_timeoutBetweenMessages) && _x.Value.Posts.Any());

                    var messageInfo = keyValue.Value;

                    if (messageInfo != null)
                    {
                        messageInfo.Used = true;
                        
                        Monitor.Exit(m_syncObject);
                        
                        if (messageInfo.Posts.TryDequeue(out var postToSend))
                        {
                            try
                            {
                                await m_sendAction(keyValue.Key, postToSend);

                                messageInfo.LastSendTime = DateTime.Now;
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Failed to send message {ex}");
                                //todo enqueue postToSend?
                            }
                        }

                        lock (m_syncObject)
                        {
                            if (messageInfo.Posts.Count == 0)
                            {
                                m_messages.Remove(keyValue.Key);
                            }
                            else
                            {
                                messageInfo.Used = false;
                            }
                        }
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
                    messagesInfo.Posts.Enqueue(_message.PostToSend);
                else
                    m_messages.Add(_message.UserId, new QueuedMessageInfo(_message.PostToSend));
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
        public bool Used { get; set; }

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
