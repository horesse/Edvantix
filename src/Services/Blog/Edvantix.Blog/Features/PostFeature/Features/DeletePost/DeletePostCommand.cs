namespace Edvantix.Blog.Features.PostFeature.Features.DeletePost;

/// <summary>
/// Команда для архивирования поста (мягкое удаление через перевод в статус Archived).
/// </summary>
public sealed record DeletePostCommand(Guid PostId) : IRequest;

/// <summary>
/// Обработчик команды удаления/архивирования поста.
/// Переводит пост в статус Archived вместо физического удаления.
/// </summary>
public sealed class DeletePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeletePostCommand>
{
    public async ValueTask<Unit> Handle(
        DeletePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        post.Archive();

        await postRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
