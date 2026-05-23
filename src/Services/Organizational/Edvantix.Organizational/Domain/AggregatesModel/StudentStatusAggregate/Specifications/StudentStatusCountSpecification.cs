namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта статусов студентов.
/// <para>
/// <paramref name="isArchived"/> = <see langword="false"/> — только активные,
/// <paramref name="isArchived"/> = <see langword="true"/> — только архивные,
/// <paramref name="isArchived"/> = <see langword="null"/> — все записи.
/// </para>
/// </summary>
public sealed class StudentStatusCountSpecification : Specification<StudentStatus>
{
    public StudentStatusCountSpecification(
        Guid organizationId,
        bool? isArchived = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(s => s.OrganizationId == organizationId);

        if (isArchived.HasValue)
            Query.Where(s => s.IsArchived == isArchived.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(s => s.Name.ToLower().Contains(term) || s.Code.ToLower().Contains(term));
        }
    }
}
