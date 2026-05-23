namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта тегов студентов организации.
/// <para>
/// <paramref name="isArchived"/> = <see langword="false"/> — только активные,
/// <paramref name="isArchived"/> = <see langword="true"/> — только архивные,
/// <paramref name="isArchived"/> = <see langword="null"/> — все записи.
/// </para>
/// </summary>
public sealed class StudentTagCountSpecification : Specification<StudentTag>
{
    public StudentTagCountSpecification(
        Guid organizationId,
        bool? isArchived = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(t => t.OrganizationId == organizationId);

        if (isArchived.HasValue)
            Query.Where(t => t.IsArchived == isArchived.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(t => t.Name.ToLower().Contains(term));
        }
    }
}
