namespace Edvantix.Organizational.Features.GroupFeature.Features.UpdateGroup;

public sealed record UpdateGroupRequest(string Name, string? Description);

public class UpdateGroupEndpoint : IEndpoint<NoContent, UpdateGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/groups/{id:long}",
                async (long id, UpdateGroupRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateGroupCommand(id, request.Name, request.Description);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateGroup")
            .WithTags("Groups")
            .WithSummary("Обновить группу")
            .WithDescription(
                "Обновляет информацию о группе. Доступно владельцу, менеджеру, учителю/менеджеру группы."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
