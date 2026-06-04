namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта тегов студентов организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные,
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные.
/// </para>
/// </summary>
public sealed class StudentTagCountSpecification : Specification<StudentTag>
{
    public StudentTagCountSpecification(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(t => t.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(t => t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(t => t.Name.ToLower().Contains(search.Trim().ToLower()));
    }
}
