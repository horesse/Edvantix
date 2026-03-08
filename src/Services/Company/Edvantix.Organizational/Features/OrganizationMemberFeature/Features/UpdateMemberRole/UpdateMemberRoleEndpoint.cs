namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.UpdateMemberRole;

public sealed record UpdateMemberRoleRequest(OrganizationRole NewRole);

public sealed class UpdateMemberRoleEndpoint : IEndpoint<NoContent, UpdateMemberRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{orgId:guid}/members/{memberId:guid}/role",
                async (
                    Guid orgId,
                    Guid memberId,
                    UpdateMemberRoleRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdateMemberRoleCommand(orgId, memberId, request.NewRole);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOrganizationMemberRole")
            .WithTags("Organization Members")
            .WithSummary("Изменить роль участника")
            .WithDescription("Изменяет роль участника организации. Доступно владельцу и менеджеру.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateMemberRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
