using Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Queries.GetCustomRole;

/// <summary>
/// Эндпоинт получения кастомной роли по идентификатору.
/// </summary>
public sealed class GetCustomRoleEndpoint
    : IEndpoint<Ok<OrganizationCustomRoleModel>, GetCustomRoleQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/custom-roles/{roleId:guid}",
                async (Guid organizationId, Guid roleId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetCustomRoleQuery(roleId, organizationId), sender, ct)
            )
            .WithName("GetCustomRole")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Получить кастомную роль")
            .WithDescription(
                "Возвращает кастомную роль организации по идентификатору. Доступно участникам организации."
            )
            .Produces<OrganizationCustomRoleModel>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationCustomRoleModel>> HandleAsync(
        GetCustomRoleQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
