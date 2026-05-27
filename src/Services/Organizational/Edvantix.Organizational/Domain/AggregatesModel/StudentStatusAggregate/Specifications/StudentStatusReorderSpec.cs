namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>Спецификация для загрузки активных статусов студентов организации с отслеживанием изменений.</summary>
public sealed class StudentStatusReorderSpec : Specification<StudentStatus>
{
    public StudentStatusReorderSpec(Guid organizationId)
    {
        Query
            .Where(s => s.OrganizationId == organizationId && !s.IsArchived)
            .OrderBy(s => s.Order)
            .AsTracking();
    }
}
