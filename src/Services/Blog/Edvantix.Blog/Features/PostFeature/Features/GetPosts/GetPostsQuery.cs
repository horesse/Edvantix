using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Persona.Grpc.Services;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPosts;

public sealed record GetPostsQuery(
    [property: Description("Фильтр по типу контента")] PostType? Type = null,
    [property: Description("Фильтр по идентификатору категории")] Guid? CategoryId = null,
    [property: Description("Фильтр по идентификатору тега")] Guid? TagId = null,
    [property: Description("Текстовый поиск по заголовку и описанию")] string? Search = null,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IQuery<PagedResult<PostSummaryModel>>;

public sealed class GetPostsQueryHandler(
    IPostRepository postRepository,
    IMapper<Post, PostSummaryModel> postSummaryMapper,
    IProfileService profileService,
    IMapper<ProfileReply, AuthorModel> authorMapper
) : IQueryHandler<GetPostsQuery, PagedResult<PostSummaryModel>>
{
    public async ValueTask<PagedResult<PostSummaryModel>> Handle(
        GetPostsQuery request,
        CancellationToken cancellationToken
    )
    {
        var countSpec = new PostSpecification(
            status: PostStatus.Published,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search
        );

        var count = await postRepository.CountAsync(countSpec, cancellationToken);

        var listSpec = new PostSpecification(
            status: PostStatus.Published,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search,
            includeRelations: true
        );

        listSpec.Skip = (request.PageIndex - 1) * request.PageSize;
        listSpec.Take = request.PageSize;

        var posts = await postRepository.ListAsync(listSpec, cancellationToken);
        var items = new List<PostSummaryModel>(posts.Count);

        foreach (var post in posts)
        {
            var item = postSummaryMapper.Map(post);
            await item.EnrichAuthor(post.AuthorId, profileService, authorMapper, cancellationToken);
            items.Add(item);
        }

        return new PagedResult<PostSummaryModel>(items, request.PageIndex, request.PageSize, count);
    }
}
