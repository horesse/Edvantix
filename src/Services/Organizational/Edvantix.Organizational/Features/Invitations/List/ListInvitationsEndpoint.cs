using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Invitations.List;

/// <summary>Возвращает постраничный список приглашений организации.</summary>
public sealed class ListInvitationsEndpoint
    : IEndpoint<Ok<IReadOnlyCollection<InvitationDto>>, ListInvitationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/invitations",
                async (
                    [AsParameters] ListInvitationsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListInvitations")
            .WithTags("Приглашения")
            .WithSummary("Список приглашений организации")
            .Produces<IReadOnlyCollection<InvitationDto>>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyCollection<InvitationDto>>> HandleAsync(
        ListInvitationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
