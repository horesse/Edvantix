namespace Edvantix.Organizational.Features.Invitations.Accept;

/// <summary>Принимает приглашение по токену из письма или in-app уведомления.</summary>
public sealed class AcceptInvitationEndpoint : IEndpoint<NoContent, string, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/invitations/{token}/accept",
                async (string token, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(token, sender, cancellationToken)
            )
            .WithName("AcceptInvitation")
            .WithTags("Приглашения")
            .WithSummary("Принять приглашение")
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
        await sender.Send(new AcceptInvitationCommand(token), cancellationToken);
        return TypedResults.NoContent();
    }
}
