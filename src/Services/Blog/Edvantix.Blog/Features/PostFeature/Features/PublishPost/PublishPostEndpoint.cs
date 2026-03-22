namespace Edvantix.Blog.Features.PostFeature.Features.PublishPost;

public sealed record PublishPostRequest(DateTime? ScheduledAt = null);

public sealed class PublishPostEndpoint : IEndpoint<NoContent, PublishPostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/posts/{postId:guid}/publish",
                async (
                    Guid postId,
                    PublishPostRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new PublishPostCommand(postId, request.ScheduledAt),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Опубликовать пост")
            .WithTags("Администрирование")
            .WithSummary("Опубликовать пост")
            .WithDescription(
                "Публикует пост немедленно или планирует его публикацию на указанное время."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        PublishPostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
