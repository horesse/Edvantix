using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.InvitationFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.InvitationFeature.Features.GetMyInvitations;

/// <summary>
/// GET /invitations/my — мои приглашения.
/// </summary>
public class GetMyInvitationsEndpoint
    : IEndpoint<Ok<IEnumerable<InvitationModel>>, GetMyInvitationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/invitations/my",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetMyInvitationsQuery(), sender, ct)
            )
            .WithName("GetMyInvitations")
            .WithTags("Invitations")
            .WithSummary("Мои приглашения")
            .WithDescription("Возвращает список ожидающих приглашений текущего пользователя.")
            .Produces<IEnumerable<InvitationModel>>()
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<InvitationModel>>> HandleAsync(
        GetMyInvitationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
