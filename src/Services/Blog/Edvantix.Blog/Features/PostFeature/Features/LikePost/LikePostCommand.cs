using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Specifications;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.PostFeature.Features.LikePost;

/// <summary>
/// Команда для постановки лайка на пост авторизованным пользователем.
/// </summary>
public sealed record LikePostCommand(long PostId) : IRequest;

/// <summary>
/// Обработчик команды постановки лайка.
/// Проверяет уникальность через репозиторий PostLike и атомарно обновляет счётчик на посте.
/// </summary>
public sealed class LikePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<LikePostCommand>
{
    public async Task Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var userId = await provider.GetProfileId(cancellationToken);

        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        if (post.Status != PostStatus.Published)
            throw new InvalidOperationException("Нельзя поставить лайк на неопубликованный пост.");

        using var likeRepo = provider.GetRequiredService<IPostLikeRepository>();

        var existingLikeSpec = new PostLikeSpecification(postId: request.PostId, userId: userId);

        var existingLike = await likeRepo.GetFirstByExpressionAsync(
            existingLikeSpec,
            cancellationToken
        );

        if (existingLike is not null)
            throw new InvalidOperationException("Пользователь уже поставил лайк на этот пост.");

        var like = new PostLike(request.PostId, userId);
        await likeRepo.InsertAsync(like, cancellationToken);
        await likeRepo.SaveEntitiesAsync(cancellationToken);

        // Обновляем денормализованный счётчик на посте
        post.IncrementLikesCount();
        await postRepo.UpdateAsync(post, cancellationToken);
        await postRepo.SaveEntitiesAsync(cancellationToken);
    }
}
