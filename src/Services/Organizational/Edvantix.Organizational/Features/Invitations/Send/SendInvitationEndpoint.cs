namespace Edvantix.Organizational.Features.Invitations.Send;

/// <summary>Отправляет приглашение в организацию (email или in-app).</summary>
public sealed class SendInvitationEndpoint
    : IEndpoint<Created<Guid>, SendInvitationCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/invitations",
                async (
                    SendInvitationCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("SendInvitation")
            .WithTags("Приглашения")
            .WithSummary("Отправить приглашение в организацию")
            .WithDescription(
                "Поддерживает два канала: Email (ссылка с токеном) и InApp (по логину пользователя)."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        SendInvitationCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetInvitationById", new { id }) ?? $"/api/invitations/{id}";
        return TypedResults.Created(location, id);
    }
}
