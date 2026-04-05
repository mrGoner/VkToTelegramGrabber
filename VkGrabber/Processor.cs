using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace VkGrabber;

internal class Processor : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Channel<ProcessorJob> _jobsChannel;
    private bool _isDisposed;

    public Processor(int maxPerformed, int processorCapacity)
    {
        _jobsChannel = Channel.CreateBounded<ProcessorJob>(new BoundedChannelOptions(processorCapacity)
        {
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true
        });

        _semaphore = new SemaphoreSlim(maxPerformed, maxPerformed);

        _ = PerformTask();
    }

    private async Task PerformTask()
    {
        await foreach (var job in _jobsChannel.Reader.ReadAllAsync())
        {
            if (_isDisposed)
                return;

            await _semaphore.WaitAsync();

            _ = job.PlannedAction().ContinueWith(_ =>
            {
                if (_isDisposed)
                    return;

                _semaphore.Release();
            });
        }
    }

    public async Task AddAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Processor));

        await _jobsChannel.Writer.WriteAsync(new ProcessorJob(action), cancellationToken);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        _jobsChannel.Writer.TryComplete();
        _semaphore.Dispose();
    }
}