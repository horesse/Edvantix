namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта уровней в справочнике.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные.
/// <paramref name="isArchive"/> = <see langword="true"/> — только деактивированные.
/// </para>
/// </summary>
public sealed class LevelCountDirectorySpec : Specification<Level>
{
    public LevelCountDirectorySpec(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId)
            .Where(l => l.IsActive == !isArchive);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(l => l.Name.Contains(search.Trim()));
    }
}
