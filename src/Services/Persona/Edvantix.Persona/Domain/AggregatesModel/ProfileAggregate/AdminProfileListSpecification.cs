namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Спецификация для постраничного получения профилей в административной панели.
/// Поддерживает фильтрацию по поисковой строке и статусу блокировки.
/// </summary>
public sealed class AdminProfileListSpecification : Specification<Profile>
{
    public AdminProfileListSpecification(
        int offset,
        int limit,
        string? search = null,
        bool? isBlocked = null
    )
    {
        Query
            .AsNoTracking()
            .OrderByDescending(p => p.LastLoginAt)
            .ThenBy(p => p.FullName.LastName)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, search, isBlocked);
    }

    internal static void ApplyFilters(
        ISpecificationBuilder<Profile> query,
        string? search,
        bool? isBlocked
    )
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLowerInvariant();
            query.Where(p =>
                p.Login.ToLower().Contains(lower)
                || p.FullName.LastName.ToLower().Contains(lower)
                || p.FullName.FirstName.ToLower().Contains(lower)
            );
        }

        if (isBlocked.HasValue)
        {
            query.Where(p => p.IsBlocked == isBlocked.Value);
        }
    }
}
