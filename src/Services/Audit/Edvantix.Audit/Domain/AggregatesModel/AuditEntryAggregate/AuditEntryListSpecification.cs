using Edvantix.Audit.Domain.Enums;

namespace Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;

/// <summary>
/// Спецификация для постраничного получения записей журнала аудита организации.
/// Поддерживает фильтрацию по типу действия, типу сущности и диапазону дат.
/// </summary>
public sealed class AuditEntryListSpecification : Specification<AuditEntry>
{
    public AuditEntryListSpecification(
        Guid organizationId,
        int offset,
        int limit,
        AuditAction? action = null,
        AuditEntityType? entityType = null,
        Guid? actorId = null,
        DateTime? from = null,
        DateTime? to = null
    )
    {
        Query
            .AsNoTracking()
            .Where(e => e.OrganizationId == organizationId)
            .OrderByDescending(e => e.OccurredAt)
            .Skip(offset)
            .Take(limit);

        if (action.HasValue)
            Query.Where(e => e.Action == action.Value);

        if (entityType.HasValue)
            Query.Where(e => e.EntityType == entityType.Value);

        if (actorId.HasValue)
            Query.Where(e => e.ActorId == actorId.Value);

        if (from.HasValue)
            Query.Where(e => e.OccurredAt >= from.Value);

        if (to.HasValue)
            Query.Where(e => e.OccurredAt <= to.Value);
    }
}
