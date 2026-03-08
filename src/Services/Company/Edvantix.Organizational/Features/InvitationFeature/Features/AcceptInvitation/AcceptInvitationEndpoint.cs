namespace Edvantix.Organizational.Features.InvitationFeature.Features.AcceptInvitation;

/// <summary>
/// POST /invitations/{token}/accept — принять приглашение.
/// </summary>
public sealed class AcceptInvitationEndpoint : IEndpoint<Ok<Guid>, AcceptInvitationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/invitations/{token:guid}/accept",
                async (Guid token, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new AcceptInvitationCommand(token), sender, ct)
            )
            .WithName("AcceptInvitation")
            .WithTags("Invitations")
            .WithSummary("Принять приглашение")
            .WithDescription(
                "Принимает приглашение в организацию по токену. Текущий пользователь становится участником."
            )
            .Produces<Guid>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Ok<Guid>> HandleAsync(
        AcceptInvitationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var memberId = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(memberId);
    }
}
