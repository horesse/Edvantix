namespace Edvantix.SharedKernel.SeedWork;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}
