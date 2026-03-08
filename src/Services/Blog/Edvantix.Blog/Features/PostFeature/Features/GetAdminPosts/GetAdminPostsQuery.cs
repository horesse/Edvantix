using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPosts;

/// <summary>
/// Административный запрос для получения всех постов с любым статусом.
/// В отличие от публичного GetPostsQuery не ограничивает выборку по статусу Published.
/// </summary>
public sealed record GetAdminPostsQuery(
    [property: Description("Фильтр по статусу поста")] PostStatus? Status = null,
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

/// <summary>
/// Обработчик административного запроса на получение постов.
/// Возвращает посты любого статуса, обогащённые данными автора из Profile.
/// </summary>
public sealed class GetAdminPostsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetAdminPostsQuery, PagedResult<PostSummaryModel>>
{
    public async ValueTask<PagedResult<PostSummaryModel>> Handle(
        GetAdminPostsQuery request,
        CancellationToken cancellationToken
    )
    {
        var countSpec = new PostSpecification(
            status: request.Status,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search
        );

        var postRepo = provider.GetRequiredService<IPostRepository>();

        var count = await postRepo.CountAsync(countSpec, cancellationToken);

        var listSpec = new PostSpecification(
            status: request.Status,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search,
            includeRelations: true
        );

        listSpec.Skip = (request.PageIndex - 1) * request.PageSize;
        listSpec.Take = request.PageSize;

        var posts = await postRepo.ListAsync(listSpec, cancellationToken);
        var mapper = provider.GetRequiredService<IMapper<Post, PostSummaryModel>>();

        var items = new List<PostSummaryModel>(posts.Count);

        foreach (var post in posts)
        {
            var item = mapper.Map(post);
            await item.EnrichAuthor(post.AuthorId, provider, cancellationToken);
            items.Add(item);
        }

        return new PagedResult<PostSummaryModel>(items, request.PageIndex, request.PageSize, count);
    }
}
