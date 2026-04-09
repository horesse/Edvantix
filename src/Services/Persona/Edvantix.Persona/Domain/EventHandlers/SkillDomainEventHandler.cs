using Edvantix.Persona.Domain.Events;

namespace Edvantix.Persona.Domain.EventHandlers;

public sealed class SkillDomainEventHandler(
    ISkillRepository skillRepository,
    ILogger<SkillDomainEventHandler> logger
) : INotificationHandler<SkillRemovedFromProfileDomainEvent>
{
    public async ValueTask Handle(
        SkillRemovedFromProfileDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var isUsed = await skillRepository.IsUsedByAnyProfileAsync(
            notification.SkillId,
            cancellationToken
        );

        if (isUsed)
            return;

        var skill = await skillRepository.FindAsync(
            new SkillSpecification(notification.SkillId),
            cancellationToken
        );

        if (skill is null)
            return;

        logger.LogInformation(
            "Навык '{SkillName}' ({SkillId}) не используется ни одним профилем — удаляется из каталога.",
            skill.Name,
            skill.Id
        );

        skillRepository.Remove(skill);
        await skillRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
