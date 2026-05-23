namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

/// <summary>
/// Спецификация для получения первых <paramref name="limit"/> ролей организации
/// в порядке создания (по UUID v7, который содержит временную метку).
/// </summary>
public sealed class RoleNamePreviewSpecification : Specification<OrganizationRole>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="limit">Максимальное количество записей (по умолчанию 5).</param>
    public RoleNamePreviewSpecification(Guid organizationId, int limit = 5)
    {
        Query
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId)
            .OrderBy(r => r.Id)
            .Take(limit);
    }
}
