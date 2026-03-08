namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.AddMember;

public sealed record AddMemberRequest(Guid ProfileId, OrganizationRole Role);

public sealed class AddMemberEndpoint : IEndpoint<Created<Guid>, AddMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:guid}/members",
                async (
                    Guid orgId,
                    AddMemberRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new AddMemberCommand(orgId, request.ProfileId, request.Role);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("AddOrganizationMember")
            .WithTags("Organization Members")
            .WithSummary("Добавить участника")
            .WithDescription(
                "Добавляет нового участника в организацию. Доступно владельцу и менеджеру."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddMemberCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{command.OrganizationId}/members/{id}", id);
    }
}
