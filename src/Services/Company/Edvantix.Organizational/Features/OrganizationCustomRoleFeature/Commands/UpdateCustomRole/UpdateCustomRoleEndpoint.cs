using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.UpdateCustomRole;

/// <summary>
/// Эндпоинт обновления кастомной роли организации.
/// </summary>
public sealed class UpdateCustomRoleEndpoint
    : IEndpoint<NoContent, UpdateCustomRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{organizationId:guid}/custom-roles/{roleId:guid}",
                async (
                    Guid organizationId,
                    Guid roleId,
                    UpdateCustomRoleRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateCustomRoleCommand(
                            roleId,
                            organizationId,
                            body.Code,
                            body.BaseRole,
                            body.Description
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateCustomRole")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Обновить кастомную роль")
            .WithDescription(
                "Обновляет кастомную роль организации. Доступно только для Owner. "
                    + "Изменение кода запрещено, если роль назначена пользователям."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateCustomRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>
/// Тело запроса обновления кастомной роли.
/// </summary>
public sealed record UpdateCustomRoleRequest(
    string Code,
    OrganizationBaseRole BaseRole,
    string? Description
);
