using System.Collections.Immutable;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.SharedKernel;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(ImmutableList<IHasDomainEvents> entitiesWithEvents);
}
