namespace Edvantix.Blog.Features.TagFeature.DeleteTag;

/// <summary>
/// Административный эндпоинт для удаления тега блога.
/// </summary>
public sealed class DeleteTagEndpoint : IEndpoint<NoContent, DeleteTagCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/admin/tags/{tagId:long}",
                async (Guid tagId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new DeleteTagCommand(tagId), sender, ct)
            )
            .WithName("DeleteTag")
            .WithTags("Admin.Tags")
            .WithSummary("Удалить тег")
            .WithDescription("Удаляет тег блога. Доступно только администраторам.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        DeleteTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
