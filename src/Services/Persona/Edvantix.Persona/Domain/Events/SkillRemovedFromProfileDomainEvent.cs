namespace Edvantix.Persona.Domain.Events;

/// <summary>
/// Доменное событие, публикуемое когда пользователь убирает навык из своего профиля.
/// Обрабатывается для очистки глобального каталога от навыков, которые больше
/// не используются ни одним профилем.
/// </summary>
public sealed class SkillRemovedFromProfileDomainEvent(Guid profileId, Guid skillId) : DomainEvent
{
    public Guid ProfileId { get; } = profileId;
    public Guid SkillId { get; } = skillId;
}
