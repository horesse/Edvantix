using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.InvitationFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.InvitationFeature.Features.GetPendingInvitations;

/// <summary>
/// GET /organizations/{orgId}/invitations — список ожидающих приглашений.
/// </summary>
public class GetPendingInvitationsEndpoint
    : IEndpoint<Ok<IEnumerable<InvitationModel>>, GetPendingInvitationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{orgId:long}/invitations",
                async (long orgId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetPendingInvitationsQuery(orgId), sender, ct)
            )
            .WithName("GetPendingInvitations")
            .WithTags("Invitations")
            .WithSummary("Список ожидающих приглашений")
            .WithDescription(
                "Возвращает список ожидающих приглашений организации. Доступно владельцу и менеджеру."
            )
            .Produces<IEnumerable<InvitationModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<InvitationModel>>> HandleAsync(
        GetPendingInvitationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
