using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.UnlikePost;

public sealed record UnlikePostCommand(Guid PostId) : ICommand;

public sealed class UnlikePostCommandHandler(
    ClaimsPrincipal claims,
    IPostRepository postRepository,
    IPostLikeRepository postLikeRepository
) : ICommandHandler<UnlikePostCommand>
{
    public async ValueTask<Unit> Handle(
        UnlikePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = claims.GetProfileIdOrError();

        var spec = new PostLikeSpecification(postId: request.PostId, userId: userId);
        var like = await postLikeRepository.Get(spec, cancellationToken);

        Guard.Against.NotFound(like, request.PostId);

        await postLikeRepository.DeleteAsync(like, cancellationToken);

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        post?.DecrementLikesCount();

        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
