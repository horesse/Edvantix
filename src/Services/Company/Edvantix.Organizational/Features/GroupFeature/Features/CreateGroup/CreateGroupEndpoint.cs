namespace Edvantix.Organizational.Features.GroupFeature.Features.CreateGroup;

public sealed record CreateGroupRequest(string Name, string? Description);

public class CreateGroupEndpoint : IEndpoint<Created<Guid>, CreateGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:long}/groups",
                async (
                    Guid orgId,
                    CreateGroupRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new CreateGroupCommand(orgId, request.Name, request.Description);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("CreateGroup")
            .WithTags("Groups")
            .WithSummary("Создать группу")
            .WithDescription(
                "Создаёт новую группу в организации. Доступно владельцу, менеджеру и учителю."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/groups/{id}", id);
    }
}
