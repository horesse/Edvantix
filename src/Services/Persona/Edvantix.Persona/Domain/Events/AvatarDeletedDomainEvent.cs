namespace Edvantix.Persona.Domain.Events;

public sealed class AvatarDeletedDomainEvent(string avatarUrn) : DomainEvent
{
    public string AvatarUrn { get; } = avatarUrn;
}
