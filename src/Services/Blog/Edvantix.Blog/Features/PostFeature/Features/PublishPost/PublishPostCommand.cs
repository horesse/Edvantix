namespace Edvantix.Blog.Features.PostFeature.Features.PublishPost;

/// <summary>
/// Команда для публикации или планирования поста.
/// При указании ScheduledAt пост переходит в статус Scheduled,
/// иначе — немедленно в статус Published.
/// </summary>
public sealed record PublishPostCommand(long PostId, DateTime? ScheduledAt = null) : IRequest;

/// <summary>
/// Обработчик команды публикации поста.
/// </summary>
public sealed class PublishPostCommandHandler(IServiceProvider provider)
    : IRequestHandler<PublishPostCommand>
{
    public async Task Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        if (request.ScheduledAt.HasValue)
        {
            post.Schedule(request.ScheduledAt.Value);
        }
        else
        {
            post.Publish();
        }

        await postRepo.UpdateAsync(post, cancellationToken);
        await postRepo.SaveEntitiesAsync(cancellationToken);
    }
}
