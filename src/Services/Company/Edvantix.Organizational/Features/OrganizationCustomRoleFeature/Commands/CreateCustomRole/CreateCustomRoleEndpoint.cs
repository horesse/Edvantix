namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.CreateCustomRole;

/// <summary>
/// Эндпоинт создания кастомной роли организации.
/// </summary>
public sealed class CreateCustomRoleEndpoint
    : IEndpoint<Created<Guid>, CreateCustomRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{organizationId:guid}/custom-roles",
                async (
                    Guid organizationId,
                    CreateCustomRoleRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new CreateCustomRoleCommand(
                            organizationId,
                            body.Code,
                            body.BaseRole,
                            body.Description
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("CreateCustomRole")
            .WithTags("OrganizationCustomRoles")
            .WithSummary("Создать кастомную роль")
            .WithDescription(
                "Создаёт новую кастомную роль в организации. Доступно только для Owner."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateCustomRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{command.OrganizationId}/custom-roles/{id}", id);
    }
}

/// <summary>
/// Тело запроса создания кастомной роли.
/// </summary>
public sealed record CreateCustomRoleRequest(
    string Code,
    Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate.OrganizationBaseRole BaseRole,
    string? Description
);
