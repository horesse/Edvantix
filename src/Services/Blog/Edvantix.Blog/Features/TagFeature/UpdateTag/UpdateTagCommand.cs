using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.TagFeature.UpdateTag;

public sealed record UpdateTagCommand(Guid TagId, string Name, string Slug) : ICommand;

public sealed class UpdateTagCommandHandler(ITagRepository tagRepository)
    : ICommandHandler<UpdateTagCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tag = await tagRepository.GetByIdAsync(request.TagId, cancellationToken);

        Guard.Against.NotFound(tag, request.TagId);

        tag.Update(request.Name, request.Slug);

        await tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
