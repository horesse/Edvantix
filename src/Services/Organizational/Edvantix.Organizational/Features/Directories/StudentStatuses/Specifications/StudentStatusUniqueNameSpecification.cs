using Edvantix.Chassis.Specification;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Specifications;

/// <summary>
/// Спецификация для проверки уникальности имени статуса среди активных записей организации.
/// </summary>
public sealed class StudentStatusUniqueNameSpecification : Specification<StudentStatus>
{
    public StudentStatusUniqueNameSpecification(
        Guid organizationId,
        string name,
        Guid? excludeId = null
    )
    {
        Query
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && !s.IsArchived && s.Name == name);

        if (excludeId.HasValue)
            Query.Where(s => s.Id != excludeId.Value);
    }
}
