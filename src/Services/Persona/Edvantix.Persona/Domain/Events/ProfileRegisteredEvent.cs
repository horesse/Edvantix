namespace Edvantix.Persona.Domain.Events;

public sealed class ProfileRegisteredEvent(Guid accountId, string login) : DomainEvent
{
    public Guid AccountId { get; } = accountId;
    public string Login { get; } = login;
}
