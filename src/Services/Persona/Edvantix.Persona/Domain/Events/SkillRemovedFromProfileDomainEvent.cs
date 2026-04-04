namespace Edvantix.Persona.Domain.Events;

public sealed class SkillRemovedFromProfileDomainEvent(Guid profileId, Guid skillId) : DomainEvent
{
    public Guid ProfileId { get; } = profileId;
    public Guid SkillId { get; } = skillId;
}
