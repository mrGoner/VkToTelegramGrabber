using System;
using System.Threading.Tasks;

namespace VkGrabber;

internal class ProcessorJob(Func<Task> action)
{
    public Guid Id => Guid.NewGuid();
    public Func<Task> PlannedAction { get; } = action;
}