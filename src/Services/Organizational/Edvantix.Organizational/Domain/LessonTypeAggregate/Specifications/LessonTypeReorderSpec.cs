namespace Edvantix.Organizational.Domain.LessonTypeAggregate.Specifications;

/// <summary>Спецификация для загрузки активных типов занятий организации с отслеживанием изменений.</summary>
public sealed class LessonTypeReorderSpec : Specification<LessonType>
{
    public LessonTypeReorderSpec(Guid organizationId)
    {
        Query
            .Where(lt => lt.OrganizationId == organizationId && !lt.IsArchived)
            .OrderBy(lt => lt.Order)
            .AsTracking();
    }
}
