using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using MediatR;

namespace Edvantix.Blog.Features.TagFeature.Features.CreateTag;

/// <summary>
/// Команда для создания нового тега блога.
/// </summary>
public sealed record CreateTagCommand(string Name, string Slug) : IRequest<long>;

/// <summary>
/// Обработчик команды создания тега.
/// </summary>
public sealed class CreateTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateTagCommand, long>
{
    public async Task<long> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag(request.Name, request.Slug);

        using var tagRepo = provider.GetRequiredService<ITagRepository>();
        await tagRepo.InsertAsync(tag, cancellationToken);
        await tagRepo.SaveEntitiesAsync(cancellationToken);

        return tag.Id;
    }
}
