namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для постраничного получения ролей организации с поддержкой текстового поиска.
/// </summary>
public sealed class RoleListSpecification : Specification<OrganizationMemberRole>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="offset">Смещение для пагинации.</param>
    /// <param name="limit">Количество записей на странице.</param>
    /// <param name="search">Подстрока для поиска по названию или описанию (регистронезависимо).</param>
    public RoleListSpecification(Guid organizationId, int offset, int limit, string? search = null)
    {
        Query
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .Include(x => x.Permissions)
            .Skip(offset)
            .Take(limit);

        if (search != null)
        {
            Query.Search(x => x.Name, search, 1);
            Query.Search(x => x.Description, search, 2);
        }
    }
}

/// <summary>
/// Спецификация для подсчёта ролей организации (без пагинации) с поддержкой текстового поиска.
/// </summary>
public sealed class RoleCountSpecification : Specification<OrganizationMemberRole>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="search">Подстрока для поиска по названию или описанию (регистронезависимо).</param>
    public RoleCountSpecification(Guid organizationId, string? search = null)
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId && !r.IsDeleted);

        if (search != null)
        {
            Query.Search(x => x.Name, search, 1);
            Query.Search(x => x.Description, search, 2);
        }
    }
}
