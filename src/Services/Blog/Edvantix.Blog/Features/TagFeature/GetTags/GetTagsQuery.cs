namespace Edvantix.Blog.Features.TagFeature.GetTags;

public sealed record GetTagsQuery : IQuery<IReadOnlyList<TagModel>>;

public sealed class GetTagsQueryHandler(ITagRepository tagRepository, IMapper<Tag, TagModel> mapper)
    : IQueryHandler<GetTagsQuery, IReadOnlyList<TagModel>>
{
    public async ValueTask<IReadOnlyList<TagModel>> Handle(
        GetTagsQuery request,
        CancellationToken cancellationToken
    )
    {
        var tags = await tagRepository.ListAsync(cancellationToken);

        return [.. tags.Select(mapper.Map)];
    }
}
