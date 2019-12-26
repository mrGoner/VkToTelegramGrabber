using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VkGrabber
{
    internal class Processor
    {
        private readonly Queue<ProcessorJob> m_queue;
        private readonly int m_maxPerformed;
        private readonly object m_syncObject = new object();
        private readonly List<Task> m_performedTasks;
        private readonly AutoResetEvent m_autoResetEvent;

        public Processor(int _maxPerformed)
        {
            m_queue = new Queue<ProcessorJob>();
            m_performedTasks = new List<Task>(_maxPerformed);
            m_maxPerformed = _maxPerformed;
            m_autoResetEvent = new AutoResetEvent(false);

            Task.Factory.StartNew(PerformTask, TaskCreationOptions.LongRunning);
        }

        private void PerformTask()
        {
            while (true)
            {
                lock (m_syncObject)
                {
                    if(m_performedTasks.Count < m_maxPerformed &&
                        m_queue.TryDequeue(out var job))
                    {
                        var task = new Task(job.PlannedAction);

                        task.ContinueWith(_x =>
                        {
                            RemoveFromPerformed(task);
                        });

                        m_performedTasks.Add(task);

                        task.Start();

                        continue;
                    }
                }

                m_autoResetEvent.WaitOne();
            }
        }

        private void RemoveFromPerformed(Task _task)
        {
            lock (m_syncObject)
            {
                m_performedTasks.Remove(_task);
            }
        }

        public void Add(Action _action)
        {
            if (_action == null)
                throw new ArgumentNullException(nameof(_action));

            lock (m_syncObject)
            {
                m_queue.Enqueue(new ProcessorJob(_action));

                m_autoResetEvent.Set();
            }
        }
    }

    internal class ProcessorJob
    {
        public Action PlannedAction { get; }

        public ProcessorJob(Action _action)
        {
            PlannedAction = _action;
        }
    }
}