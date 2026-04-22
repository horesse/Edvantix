namespace Edvantix.Organizational.Features.Invitations.Get;

/// <summary>Возвращает приглашение по Id.</summary>
public sealed class GetInvitationEndpoint : IEndpoint<Ok<InvitationDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/invitations/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetInvitationById")
            .WithTags("Приглашения")
            .WithSummary("Получить приглашение по Id")
            .Produces<InvitationDto>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<InvitationDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetInvitationQuery(id), cancellationToken);
        return TypedResults.Ok(result);
    }
}
