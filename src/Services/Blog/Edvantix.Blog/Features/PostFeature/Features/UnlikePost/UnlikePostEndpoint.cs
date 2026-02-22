namespace Edvantix.Blog.Features.PostFeature.Features.UnlikePost;

/// <summary>
/// Эндпоинт для снятия лайка с поста.
/// </summary>
public sealed class UnlikePostEndpoint : IEndpoint<NoContent, UnlikePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/posts/{postId:long}/like",
                async (Guid postId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new UnlikePostCommand(postId), sender, ct)
            )
            .WithName("UnlikePost")
            .WithTags("Posts")
            .WithSummary("Убрать лайк")
            .WithDescription("Снимает лайк пользователя с поста.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UnlikePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
