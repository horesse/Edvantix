namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>Спецификация для загрузки активных тегов студентов организации с отслеживанием изменений.</summary>
public sealed class StudentTagReorderSpec : Specification<StudentTag>
{
    public StudentTagReorderSpec(Guid organizationId)
    {
        Query
            .Where(t => t.OrganizationId == organizationId && !t.IsArchived)
            .OrderBy(t => t.Order)
            .AsTracking();
    }
}
