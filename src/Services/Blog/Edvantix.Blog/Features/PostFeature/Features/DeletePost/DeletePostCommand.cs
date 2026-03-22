using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.DeletePost;

public sealed record DeletePostCommand(Guid PostId) : ICommand;

public sealed class DeletePostCommandHandler(IPostRepository postRepository)
    : ICommandHandler<DeletePostCommand>
{
    public async ValueTask<Unit> Handle(
        DeletePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        post.Archive();

        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
