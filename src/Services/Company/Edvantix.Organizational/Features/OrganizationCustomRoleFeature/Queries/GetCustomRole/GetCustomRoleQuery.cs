using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetCustomRole;

/// <summary>
/// Запрос кастомной роли по идентификатору.
/// </summary>
public sealed record GetCustomRoleQuery(Guid RoleId, Guid OrganizationId)
    : IQuery<OrganizationCustomRoleModel>;

/// <summary>
/// Обработчик запроса кастомной роли по идентификатору.
/// </summary>
public sealed class GetCustomRoleQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetCustomRoleQuery, OrganizationCustomRoleModel>
{
    public async ValueTask<OrganizationCustomRoleModel> Handle(
        GetCustomRoleQuery request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        var role = await service.GetByIdAsync(request.RoleId, request.OrganizationId, cancellationToken);

        return new OrganizationCustomRoleModel
        {
            Id = role.Id,
            OrganizationId = role.OrganizationId,
            Code = role.Code,
            Description = role.Description,
            BaseRole = role.BaseRole,
        };
    }
}
