using System;
using System.Threading.Tasks;

namespace VkGrabber;

internal class ProcessorJob
{
    public Guid Id => Guid.NewGuid();
    public Func<Task> PlannedAction { get; }

    public ProcessorJob(Func<Task> _action)
    {
        PlannedAction = _action;
    }
}