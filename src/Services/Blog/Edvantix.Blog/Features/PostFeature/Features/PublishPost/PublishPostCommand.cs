namespace Edvantix.Blog.Features.PostFeature.Features.PublishPost;
using Mediator;

/// <summary>
/// Команда для публикации или планирования поста.
/// При указании ScheduledAt пост переходит в статус Scheduled,
/// иначе — немедленно в статус Published.
/// </summary>
public sealed record PublishPostCommand(Guid PostId, DateTime? ScheduledAt = null) : ICommand;

/// <summary>
/// Обработчик команды публикации поста.
/// </summary>
public sealed class PublishPostCommandHandler(IServiceProvider provider)
    : ICommandHandler<PublishPostCommand>
{
    public async ValueTask<Unit> Handle(
        PublishPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var postRepo = provider.GetRequiredService<IPostRepository>();

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

        await postRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
