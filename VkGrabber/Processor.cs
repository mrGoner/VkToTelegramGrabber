using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace VkGrabber
{
    internal class Processor : IDisposable
    {
        private readonly SemaphoreSlim m_semaphore;
        private readonly Channel<ProcessorJob> m_processorJobs;
        private bool m_isDisposed;

        public Processor(int _maxPerformed, int _processorCapacity)
        {
            m_processorJobs = Channel.CreateBounded<ProcessorJob>(new BoundedChannelOptions(_processorCapacity){
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = true
            });
            
            m_semaphore = new SemaphoreSlim(_maxPerformed, _maxPerformed);

            _ = PerformTask();
        }

        private async Task PerformTask()
        {
            await foreach (var job in m_processorJobs.Reader.ReadAllAsync())
            {
                if(m_isDisposed)
                    return;

                await m_semaphore.WaitAsync();

               _ = job.PlannedAction().ContinueWith(_ =>
               {
                    if(m_isDisposed)
                        return;
                        
                    m_semaphore.Release();
               });
            }
        }

        public async Task AddAsync(Func<Task> _action, CancellationToken _cancellationToken)
        {
            if (_action == null)
                throw new ArgumentNullException(nameof(_action));

            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(Processor));

            await m_processorJobs.Writer.WriteAsync(new ProcessorJob(_action), _cancellationToken);
        }

        public void Dispose()
        {
            if (m_isDisposed)
                return;

            m_isDisposed = true;
            
            m_processorJobs.Writer.Complete();
            m_semaphore.Dispose();
        }
    }

    internal class ProcessorJob
    {
        public Guid Id => Guid.NewGuid();
        public Func<Task> PlannedAction { get; }

        public ProcessorJob(Func<Task> _action)
        {
            PlannedAction = _action;
        }
    }
}