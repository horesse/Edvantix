using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetOrganizationCustomRoles;

/// <summary>
/// Запрос списка активных кастомных ролей организации.
/// </summary>
public sealed record GetOrganizationCustomRolesQuery(Guid OrganizationId)
    : IQuery<IReadOnlyList<OrganizationCustomRoleModel>>;

/// <summary>
/// Обработчик запроса списка кастомных ролей.
/// </summary>
public sealed class GetOrganizationCustomRolesQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetOrganizationCustomRolesQuery, IReadOnlyList<OrganizationCustomRoleModel>>
{
    public async ValueTask<IReadOnlyList<OrganizationCustomRoleModel>> Handle(
        GetOrganizationCustomRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        var roles = await service.ListAsync(request.OrganizationId, cancellationToken);

        return roles
            .Select(r => new OrganizationCustomRoleModel
            {
                Id = r.Id,
                OrganizationId = r.OrganizationId,
                Code = r.Code,
                Description = r.Description,
                BaseRole = r.BaseRole,
            })
            .ToList();
    }
}
