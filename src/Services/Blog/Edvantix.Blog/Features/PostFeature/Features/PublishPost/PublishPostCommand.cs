using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.PublishPost;

public sealed record PublishPostCommand(Guid PostId, DateTime? ScheduledAt = null) : ICommand;

public sealed class PublishPostCommandHandler(IPostRepository postRepository)
    : ICommandHandler<PublishPostCommand>
{
    public async ValueTask<Unit> Handle(
        PublishPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        if (request.ScheduledAt.HasValue)
        {
            post.Schedule(request.ScheduledAt.Value);
        }
        else
        {
            post.Publish();
        }

        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
