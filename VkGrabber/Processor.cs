using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VkGrabber
{
    internal class Processor
    {
        private readonly Queue<Task> m_queue;
        private readonly int m_maxPerformed;
        private readonly object m_syncObject = new object();
        private readonly List<Task> m_performedTasks;

        public Processor(int _maxPerformed)
        {
            m_queue = new Queue<Task>();
            m_performedTasks = new List<Task>();
            m_maxPerformed = _maxPerformed;
        }

        private void PerformTask()
        {
            lock (m_syncObject)
            {
                while (m_queue.Count > 0 && m_performedTasks.Count < m_maxPerformed)
                {
                    var task = m_queue.Dequeue();

                    task.ContinueWith((arg) =>
                    {
                        lock (m_syncObject)
                        {
                            m_performedTasks.Remove(task);
                            PerformTask();
                        }
                    });

                    m_performedTasks.Add(task);

                    task.Start();
                }
            }
        }

        public void Add(Action _action)
        {
            lock (m_syncObject)
            {
                m_queue.Enqueue(new Task(_action.Invoke));
                PerformTask();
            }
        }
    }
}