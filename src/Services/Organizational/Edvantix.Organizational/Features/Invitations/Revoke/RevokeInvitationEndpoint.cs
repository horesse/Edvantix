namespace Edvantix.Organizational.Features.Invitations.Revoke;

/// <summary>Отзывает ожидающее приглашение по его Id.</summary>
public sealed class RevokeInvitationEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/invitations/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RevokeInvitation")
            .WithTags("Приглашения")
            .WithSummary("Отозвать приглашение")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new RevokeInvitationCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
