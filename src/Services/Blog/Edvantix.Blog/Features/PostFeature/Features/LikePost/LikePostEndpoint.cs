namespace Edvantix.Blog.Features.PostFeature.Features.LikePost;

public sealed class LikePostEndpoint : IEndpoint<NoContent, LikePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/posts/{postId:guid}/like",
                async (Guid postId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new LikePostCommand(postId), sender, cancellationToken)
            )
            .WithName("Поставить лайк")
            .WithTags("Посты")
            .WithSummary("Поставить лайк")
            .WithDescription("Ставит лайк на пост. Один пользователь — один лайк.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        LikePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
