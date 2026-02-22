namespace Edvantix.Blog.Features.TagFeature.CreateTag;

/// <summary>
/// Команда для создания нового тега блога.
/// </summary>
public sealed record CreateTagCommand(string Name, string Slug) : IRequest<Guid>;

/// <summary>
/// Обработчик команды создания тега.
/// </summary>
public sealed class CreateTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateTagCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tag = new Tag(request.Name, request.Slug);

        var tagRepo = provider.GetRequiredService<ITagRepository>();
        await tagRepo.AddAsync(tag, cancellationToken);
        await tagRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return tag.Id;
    }
}
