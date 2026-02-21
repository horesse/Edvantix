namespace Edvantix.Organizational.Features.GroupFeature.Features.DeleteGroup;

public class DeleteGroupEndpoint : IEndpoint<NoContent, DeleteGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/groups/{id:long}",
                async (long id, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new DeleteGroupCommand(id), sender, ct)
            )
            .WithName("DeleteGroup")
            .WithTags("Groups")
            .WithSummary("Удалить группу")
            .WithDescription(
                "Удаляет группу (мягкое удаление). Доступно владельцу и менеджеру организации."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeleteGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
