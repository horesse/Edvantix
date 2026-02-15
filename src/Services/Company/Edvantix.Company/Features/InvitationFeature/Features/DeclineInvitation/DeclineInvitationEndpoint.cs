using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.InvitationFeature.Features.DeclineInvitation;

/// <summary>
/// POST /invitations/{token}/decline — отклонить приглашение.
/// </summary>
public class DeclineInvitationEndpoint : IEndpoint<NoContent, DeclineInvitationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/invitations/{token:guid}/decline",
                async (Guid token, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new DeclineInvitationCommand(token), sender, ct)
            )
            .WithName("DeclineInvitation")
            .WithTags("Invitations")
            .WithSummary("Отклонить приглашение")
            .WithDescription("Отклоняет приглашение в организацию по токену.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeclineInvitationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
