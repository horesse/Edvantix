using Edvantix.Blog.Features.CategoryFeature;
using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Features.TagFeature;
using Edvantix.Blog.Grpc.Services;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPosts;

/// <summary>
/// Административный запрос для получения всех постов с любым статусом.
/// В отличие от публичного GetPostsQuery не ограничивает выборку по статусу Published.
/// </summary>
public sealed record GetAdminPostsQuery(
    [property: Description("Фильтр по статусу поста")] PostStatus? Status = null,
    [property: Description("Фильтр по типу контента")] PostType? Type = null,
    [property: Description("Фильтр по идентификатору категории")] ulong? CategoryId = null,
    [property: Description("Фильтр по идентификатору тега")] ulong? TagId = null,
    [property: Description("Текстовый поиск по заголовку и описанию")] string? Search = null,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IRequest<PagedResult<PostSummaryModel>>;

/// <summary>
/// Обработчик административного запроса на получение постов.
/// Возвращает посты любого статуса, обогащённые данными автора из Profile.
/// </summary>
public sealed class GetAdminPostsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetAdminPostsQuery, PagedResult<PostSummaryModel>>
{
    public async ValueTask<PagedResult<PostSummaryModel>> Handle(
        GetAdminPostsQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = new PostSpecification(
            status: request.Status,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search
        );

        var postRepo = provider.GetRequiredService<IPostRepository>();

        var count = await postRepo.CountAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var posts = await postRepo.ListAsync(spec, cancellationToken);

        var profileService = provider.GetRequiredService<IProfileService>();

        var items = new List<PostSummaryModel>(posts.Count);

        foreach (var post in posts)
        {
            var author = await profileService.GetAuthorById(post.AuthorId, cancellationToken);

            items.Add(
                new PostSummaryModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Slug = post.Slug,
                    Summary = post.Summary,
                    Status = post.Status,
                    Type = post.Type,
                    IsPremium = post.IsPremium,
                    CoverImageUrl = post.CoverImageUrl,
                    LikesCount = post.LikesCount,
                    PublishedAt = post.PublishedAt,
                    ScheduledAt = post.ScheduledAt,
                    Author = author is null
                        ? null
                        : new AuthorModel { Id = author.Id, FullName = author.FullName },
                    Categories = post
                        .Categories.Select(c => new CategoryModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Slug = c.Slug,
                        })
                        .ToList(),
                    Tags =
                    [
                        .. post.Tags.Select(t => new TagModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug,
                        }),
                    ],
                }
            );
        }

        return new PagedResult<PostSummaryModel>(items, request.PageIndex, request.PageSize, count);
    }
}
