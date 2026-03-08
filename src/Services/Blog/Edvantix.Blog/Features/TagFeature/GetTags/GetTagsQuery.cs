namespace Edvantix.Blog.Features.TagFeature.GetTags;

using Mediator;

public sealed record GetTagsQuery : IQuery<IReadOnlyList<TagModel>>;

public sealed class GetTagsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetTagsQuery, IReadOnlyList<TagModel>>
{
    public async ValueTask<IReadOnlyList<TagModel>> Handle(
        GetTagsQuery request,
        CancellationToken cancellationToken
    )
    {
        var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tags = await tagRepo.ListAsync(cancellationToken);

        var mapper = provider.GetRequiredService<IMapper<Tag, TagModel>>();

        return [.. tags.Select(mapper.Map)];
    }
}
