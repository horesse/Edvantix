using Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;
using Edvantix.Persona.Domain.Events;

namespace Edvantix.Persona.Domain.EventHandlers;

/// <summary>
/// Обрабатывает доменное событие удаления навыка из профиля.
/// Удаляет навык из глобального каталога, если он больше не используется ни одним профилем.
/// Событие срабатывает ПОСЛЕ сохранения профиля, поэтому проверка корректна.
/// </summary>
public sealed class SkillDomainEventHandler(
    ISkillRepository skillRepository,
    ILogger<SkillDomainEventHandler> logger
) : INotificationHandler<SkillRemovedFromProfileDomainEvent>
{
    public async ValueTask Handle(
        SkillRemovedFromProfileDomainEvent notification,
        CancellationToken ct
    )
    {
        var isUsed = await skillRepository.IsUsedByAnyProfileAsync(notification.SkillId, ct);

        if (isUsed)
            return;

        var skill = await skillRepository.FindAsync(new SkillByIdSpec(notification.SkillId), ct);

        if (skill is null)
            return;

        logger.LogInformation(
            "Навык '{SkillName}' ({SkillId}) не используется ни одним профилем — удаляется из каталога.",
            skill.Name,
            skill.Id
        );

        skillRepository.Remove(skill);
        await skillRepository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
