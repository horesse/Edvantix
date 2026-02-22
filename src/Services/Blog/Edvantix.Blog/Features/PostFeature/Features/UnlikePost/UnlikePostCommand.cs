using Edvantix.Blog.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features.UnlikePost;

/// <summary>
/// Команда для снятия лайка с поста.
/// </summary>
public sealed record UnlikePostCommand(long PostId) : IRequest;

/// <summary>
/// Обработчик команды снятия лайка.
/// Находит лайк через репозиторий, удаляет его и уменьшает счётчик на посте.
/// </summary>
public sealed class UnlikePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<UnlikePostCommand>
{
    public async Task Handle(UnlikePostCommand request, CancellationToken cancellationToken)
    {
        var userId = await provider.GetProfileId(cancellationToken);

        using var likeRepo = provider.GetRequiredService<IPostLikeRepository>();

        var spec = new PostLikeSpecification(postId: request.PostId, userId: userId);

        var like =
            await likeRepo.GetFirstByExpressionAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Лайк не найден.");

        await likeRepo.DeleteAsync(like, cancellationToken);
        await likeRepo.SaveEntitiesAsync(cancellationToken);

        // Уменьшаем денормализованный счётчик лайков на посте
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post = await postRepo.GetByIdAsync(request.PostId, cancellationToken);

        if (post is not null)
        {
            post.DecrementLikesCount();
            await postRepo.UpdateAsync(post, cancellationToken);
            await postRepo.SaveEntitiesAsync(cancellationToken);
        }
    }
}
