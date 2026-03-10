using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetOrganizationCustomRoles;

/// <summary>
/// Запрос списка ролей организации: системные базовые + кастомные.
/// </summary>
public sealed record GetOrganizationCustomRolesQuery(Guid OrganizationId)
    : IQuery<OrganizationRolesResponse>;

/// <summary>
/// Обработчик запроса. Возвращает объединённый ответ с базовыми и кастомными ролями.
/// </summary>
public sealed class GetOrganizationCustomRolesQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetOrganizationCustomRolesQuery, OrganizationRolesResponse>
{
    public async ValueTask<OrganizationRolesResponse> Handle(
        GetOrganizationCustomRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        var customRoles = await service.ListAsync(request.OrganizationId, cancellationToken);

        var customRoleModels = customRoles
            .Select(r => new OrganizationCustomRoleModel
            {
                Id = r.Id,
                OrganizationId = r.OrganizationId,
                Code = r.Code,
                Description = r.Description,
                BaseRole = r.BaseRole,
            })
            .ToList();

        return new OrganizationRolesResponse(BaseRoleModel.All, customRoleModels);
    }
}
