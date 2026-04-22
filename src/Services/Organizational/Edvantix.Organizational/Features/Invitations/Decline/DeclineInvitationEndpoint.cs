namespace Edvantix.Organizational.Features.Invitations.Decline;

/// <summary>Отклоняет приглашение по токену.</summary>
public sealed class DeclineInvitationEndpoint : IEndpoint<NoContent, string, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/invitations/{token}/decline",
                async (string token, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(token, sender, cancellationToken)
            )
            .WithName("DeclineInvitation")
            .WithTags("Приглашения")
            .WithSummary("Отклонить приглашение")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        string token,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new DeclineInvitationCommand(token), cancellationToken);
        return TypedResults.NoContent();
    }
}
