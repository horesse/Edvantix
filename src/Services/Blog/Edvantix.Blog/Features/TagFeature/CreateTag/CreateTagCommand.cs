namespace Edvantix.Blog.Features.TagFeature.CreateTag;

public sealed record CreateTagCommand(string Name, string Slug) : ICommand<Guid>;

public sealed class CreateTagCommandHandler(ITagRepository tagRepository)
    : ICommandHandler<CreateTagCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tag = new Tag(request.Name, request.Slug);

        await tagRepository.AddAsync(tag, cancellationToken);
        await tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return tag.Id;
    }
}
