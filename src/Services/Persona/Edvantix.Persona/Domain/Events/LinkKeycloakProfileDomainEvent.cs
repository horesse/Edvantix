namespace Edvantix.Persona.Domain.Events;

public sealed class LinkKeycloakProfileDomainEvent(Guid profileId, Guid accountId) : DomainEvent
{
    public Guid ProfileId { get; } = profileId;
    public Guid AccountId { get; } = accountId;
}
