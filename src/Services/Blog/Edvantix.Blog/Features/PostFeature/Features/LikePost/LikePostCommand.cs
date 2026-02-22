using Edvantix.Blog.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features.LikePost;

/// <summary>
/// Команда для постановки лайка на пост авторизованным пользователем.
/// </summary>
public sealed record LikePostCommand(Guid PostId) : IRequest;

/// <summary>
/// Обработчик команды постановки лайка.
/// Проверяет уникальность через репозиторий PostLike и атомарно обновляет счётчик на посте.
/// </summary>
public sealed class LikePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<LikePostCommand>
{
    public async ValueTask<Unit> Handle(
        LikePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = await provider.GetProfileId(cancellationToken);

        var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        if (post.Status != PostStatus.Published)
            throw new InvalidOperationException("Нельзя поставить лайк на неопубликованный пост.");

        var likeRepo = provider.GetRequiredService<IPostLikeRepository>();

        var existingLikeSpec = new PostLikeSpecification(postId: request.PostId, userId: userId);

        var existingLike = await likeRepo.Get(existingLikeSpec, cancellationToken);

        if (existingLike is not null)
            throw new InvalidOperationException("Пользователь уже поставил лайк на этот пост.");

        var like = new PostLike(request.PostId, userId);
        await likeRepo.AddAsync(like, cancellationToken);

        // Обновляем денормализованный счётчик на посте
        post.IncrementLikesCount();
        await postRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
