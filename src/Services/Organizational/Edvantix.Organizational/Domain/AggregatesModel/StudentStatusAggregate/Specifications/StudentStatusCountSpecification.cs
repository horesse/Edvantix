namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта статусов студентов.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные,
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные.
/// </para>
/// </summary>
public sealed class StudentStatusCountSpecification : Specification<StudentStatus>
{
    public StudentStatusCountSpecification(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(s => s.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(s => s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(s => s.Name.ToLower().Contains(term) || s.Code.ToLower().Contains(term));
        }
    }
}
