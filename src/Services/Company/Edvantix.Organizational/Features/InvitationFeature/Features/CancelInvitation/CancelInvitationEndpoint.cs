namespace Edvantix.Organizational.Features.InvitationFeature.Features.CancelInvitation;

/// <summary>
/// DELETE /organizations/{orgId}/invitations/{invitationId} — отменить приглашение.
/// </summary>
public class CancelInvitationEndpoint : IEndpoint<NoContent, CancelInvitationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{orgId:long}/invitations/{invitationId:guid}",
                async (long orgId, Guid invitationId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new CancelInvitationCommand(orgId, invitationId), sender, ct)
            )
            .WithName("CancelInvitation")
            .WithTags("Invitations")
            .WithSummary("Отменить приглашение")
            .WithDescription("Отменяет ожидающее приглашение. Доступно владельцу и менеджеру.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        CancelInvitationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
