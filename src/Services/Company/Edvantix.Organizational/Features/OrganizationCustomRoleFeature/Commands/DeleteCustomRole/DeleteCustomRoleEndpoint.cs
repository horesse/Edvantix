namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.DeleteCustomRole;

/// <summary>
/// Эндпоинт удаления кастомной роли организации.
/// </summary>
public sealed class DeleteCustomRoleEndpoint
    : IEndpoint<NoContent, DeleteCustomRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{organizationId:guid}/roles/{roleId:guid}",
                async (Guid organizationId, Guid roleId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(
                        new DeleteCustomRoleCommand(roleId, organizationId),
                        sender,
                        ct
                    )
            )
            .WithName("DeleteOrganizationRole")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Удалить кастомную роль")
            .WithDescription(
                "Мягко удаляет кастомную роль организации. Доступно только для Owner. "
                    + "Удаление запрещено, если роль назначена активным пользователям."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeleteCustomRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
