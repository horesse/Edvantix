namespace Edvantix.Organizational.Features.InvitationFeature.Features.CreateInvitation;

/// <summary>
/// Тело запроса на создание приглашения.
/// </summary>
public sealed record CreateInvitationRequest(
    string? InviteeEmail,
    Guid? InviteeProfileId,
    OrganizationRole Role,
    int TtlDays = 7
);

/// <summary>
/// POST /organizations/{orgId}/invitations — создать приглашение.
/// </summary>
public sealed class CreateInvitationEndpoint : IEndpoint<Created<Guid>, CreateInvitationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:long}/invitations",
                async (
                    Guid orgId,
                    CreateInvitationRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new CreateInvitationCommand(
                        orgId,
                        request.InviteeEmail,
                        request.InviteeProfileId,
                        request.Role,
                        request.TtlDays
                    );

                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("CreateInvitation")
            .WithTags("Invitations")
            .WithSummary("Создать приглашение")
            .WithDescription("Создаёт приглашение в организацию. Доступно владельцу и менеджеру.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateInvitationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created(
            $"/organizations/{command.OrganizationId}/invitations/{id}",
            id
        );
    }
}
