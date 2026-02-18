using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.Blog.Features.TagFeature.Models;
using MediatR;

namespace Edvantix.Blog.Features.TagFeature.Features.GetTags;

/// <summary>
/// Запрос для получения полного списка тегов блога.
/// </summary>
public sealed record GetTagsQuery : IRequest<IReadOnlyList<TagModel>>;

/// <summary>
/// Обработчик запроса на получение всех тегов.
/// </summary>
public sealed class GetTagsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetTagsQuery, IReadOnlyList<TagModel>>
{
    public async Task<IReadOnlyList<TagModel>> Handle(
        GetTagsQuery request,
        CancellationToken cancellationToken
    )
    {
        using var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tags = await tagRepo.GetAllAsync(cancellationToken);

        return tags.Select(t => new TagModel
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                CreatedAt = t.CreatedAt,
            })
            .ToList();
    }
}
