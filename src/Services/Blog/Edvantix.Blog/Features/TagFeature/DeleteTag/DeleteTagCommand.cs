using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.TagFeature.DeleteTag;

public sealed record DeleteTagCommand(Guid TagId) : ICommand;

public sealed class DeleteTagCommandHandler(ITagRepository tagRepository)
    : ICommandHandler<DeleteTagCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tag = await tagRepository.GetByIdAsync(request.TagId, cancellationToken);

        Guard.Against.NotFound(tag, request.TagId);

        await tagRepository.DeleteAsync(tag, cancellationToken);
        await tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
