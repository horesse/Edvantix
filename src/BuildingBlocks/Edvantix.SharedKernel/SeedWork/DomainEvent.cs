using Edvantix.SharedKernel.Helpers;
using Mediator;

namespace Edvantix.SharedKernel.SeedWork;

public abstract class DomainEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTimeHelper.UtcNow();
}
