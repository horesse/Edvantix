namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности кода статуса среди активных записей организации.
/// </summary>
public sealed class StudentStatusUniqueCodeSpecification : Specification<StudentStatus>
{
    public StudentStatusUniqueCodeSpecification(
        Guid organizationId,
        string code,
        Guid? excludeId = null
    )
    {
        Query
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && !s.IsArchived && s.Code == code);

        if (excludeId.HasValue)
            Query.Where(s => s.Id != excludeId.Value);
    }
}
