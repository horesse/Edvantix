namespace Edvantix.Blog.Features.PostFeature.Features.UnlikePost;

public sealed class UnlikePostEndpoint : IEndpoint<NoContent, UnlikePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/posts/{postId:guid}/like",
                async (Guid postId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new UnlikePostCommand(postId), sender, cancellationToken)
            )
            .WithName("Убрать лайк")
            .WithTags("Посты")
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
