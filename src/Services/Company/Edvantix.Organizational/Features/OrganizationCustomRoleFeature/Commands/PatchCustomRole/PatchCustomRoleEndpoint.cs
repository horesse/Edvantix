using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.PatchCustomRole;

/// <summary>
/// Эндпоинт частичного обновления кастомной роли организации (PATCH).
/// Позволяет изменить базовую роль и описание. Код роли не изменяется.
/// </summary>
public sealed class PatchCustomRoleEndpoint : IEndpoint<NoContent, PatchCustomRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/organizations/{organizationId:guid}/roles/{roleId:guid}",
                async (
                    Guid organizationId,
                    Guid roleId,
                    PatchCustomRoleRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new PatchCustomRoleCommand(roleId, organizationId, body.BaseRole, body.Description),
                        sender,
                        ct
                    )
            )
            .WithName("PatchOrganizationRole")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Обновить кастомную роль")
            .WithDescription(
                "Изменяет базовую роль и описание кастомной роли организации. "
                    + "Код роли не изменяется. Доступно только для Owner."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        PatchCustomRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>
/// Тело запроса частичного обновления кастомной роли.
/// </summary>
public sealed record PatchCustomRoleRequest(OrganizationBaseRole BaseRole, string? Description);
