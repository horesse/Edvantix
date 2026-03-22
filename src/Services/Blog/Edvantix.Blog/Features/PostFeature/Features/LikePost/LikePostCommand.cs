using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.LikePost;

public sealed record LikePostCommand(Guid PostId) : ICommand;

public sealed class LikePostCommandHandler(
    ClaimsPrincipal claims,
    IPostRepository postRepository,
    IPostLikeRepository postLikeRepository
) : ICommandHandler<LikePostCommand>
{
    public async ValueTask<Unit> Handle(
        LikePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = claims.GetProfileIdOrError();

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        if (post.Status != PostStatus.Published)
            throw new InvalidOperationException("Нельзя поставить лайк на неопубликованный пост.");

        var existingLikeSpec = new PostLikeSpecification(postId: request.PostId, userId: userId);
        var existingLike = await postLikeRepository.Get(existingLikeSpec, cancellationToken);

        if (existingLike is not null)
            throw new InvalidOperationException("Пользователь уже поставил лайк на этот пост.");

        var like = new PostLike(request.PostId, userId);
        await postLikeRepository.AddAsync(like, cancellationToken);

        post.IncrementLikesCount();
        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
