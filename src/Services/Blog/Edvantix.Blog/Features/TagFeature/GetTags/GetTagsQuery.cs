namespace Edvantix.Blog.Features.TagFeature.GetTags;

public sealed record GetTagsQuery : IRequest<IReadOnlyList<TagModel>>;

public sealed class GetTagsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetTagsQuery, IReadOnlyList<TagModel>>
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
