namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности названия тега среди активных записей организации.
/// </summary>
public sealed class StudentTagUniqueNameSpecification : Specification<StudentTag>
{
    public StudentTagUniqueNameSpecification(Guid organizationId, string name, Guid? excludeId = null)
    {
        Query
            .AsNoTracking()
            .Where(t => t.OrganizationId == organizationId && !t.IsArchived && t.Name == name);

        if (excludeId.HasValue)
            Query.Where(t => t.Id != excludeId.Value);
    }
}
