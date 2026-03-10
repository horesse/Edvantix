using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetOrganizationCustomRoles;

/// <summary>
/// Эндпоинт получения ролей организации (базовые + кастомные).
/// </summary>
public sealed class GetOrganizationCustomRolesEndpoint
    : IEndpoint<Ok<OrganizationRolesResponse>, GetOrganizationCustomRolesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/roles",
                async (Guid organizationId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(
                        new GetOrganizationCustomRolesQuery(organizationId),
                        sender,
                        ct
                    )
            )
            .WithName("GetOrganizationRoles")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Роли организации")
            .WithDescription(
                "Возвращает системные базовые роли и кастомные роли организации. "
                    + "Доступно участникам организации."
            )
            .Produces<OrganizationRolesResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationRolesResponse>> HandleAsync(
        GetOrganizationCustomRolesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
