namespace Edvantix.Persona.Domain.Events;

public sealed class ProfileRegisteredEvent(Guid profileId, Guid accountId, string login)
    : DomainEvent
{
    public Guid ProfileId { get; } = profileId;
    public Guid AccountId { get; } = accountId;
    public string Login { get; } = login;
}
