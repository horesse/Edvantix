using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetOrganizationCustomRoles;

/// <summary>
/// Эндпоинт получения списка кастомных ролей организации.
/// </summary>
public sealed class GetOrganizationCustomRolesEndpoint
    : IEndpoint<Ok<IReadOnlyList<OrganizationCustomRoleModel>>, GetOrganizationCustomRolesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/custom-roles",
                async (Guid organizationId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOrganizationCustomRolesQuery(organizationId), sender, ct)
            )
            .WithName("GetOrganizationCustomRoles")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Список кастомных ролей организации")
            .WithDescription(
                "Возвращает все активные кастомные роли организации. Доступно участникам организации."
            )
            .Produces<IReadOnlyList<OrganizationCustomRoleModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<OrganizationCustomRoleModel>>> HandleAsync(
        GetOrganizationCustomRolesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
