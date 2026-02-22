namespace Edvantix.Blog.Domain.Events;

/// <summary>
/// Обработчик доменного события публикации поста.
/// Пока только логирует факт публикации.
/// </summary>
public sealed class PostPublishedEventHandler(ILogger<PostPublishedEventHandler> logger)
    : INotificationHandler<PostPublishedEvent>
{
    public ValueTask Handle(
        PostPublishedEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Post published: PostId={PostId}, Slug={Slug}, AuthorId={AuthorId}",
            notification.PostId,
            notification.Slug,
            notification.AuthorId
        );

        return ValueTask.CompletedTask;
    }
}
