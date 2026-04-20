using Edvantix.SharedKernel.Helpers;

namespace Edvantix.SharedKernel.SeedWork;

public abstract class DomainEvent
{
    public DateTime DateOccurred { get; protected set; } = DateTimeHelper.UtcNow();
}
