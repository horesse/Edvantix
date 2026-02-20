using System.ComponentModel;
using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Specifications;
using Edvantix.Blog.Features.CategoryFeature.Models;
using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Features.TagFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Constants.Core;
using Edvantix.SharedKernel.Results;
using MediatR;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPosts;

/// <summary>
/// Запрос для получения пагинированного списка опубликованных постов с фильтрацией.
/// </summary>
public sealed record GetPostsQuery(
    [property: Description("Фильтр по типу контента")] PostType? Type = null,
    [property: Description("Фильтр по идентификатору категории")] long? CategoryId = null,
    [property: Description("Фильтр по идентификатору тега")] long? TagId = null,
    [property: Description("Текстовый поиск по заголовку и описанию")] string? Search = null,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IRequest<PagedResult<PostSummaryModel>>;

/// <summary>
/// Обработчик запроса на получение списка постов.
/// Возвращает только опубликованные посты, обогащённые данными автора из Profile.
/// </summary>
public sealed class GetPostsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetPostsQuery, PagedResult<PostSummaryModel>>
{
    public async Task<PagedResult<PostSummaryModel>> Handle(
        GetPostsQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = new PostSpecification(
            status: PostStatus.Published,
            type: request.Type,
            categoryId: request.CategoryId,
            tagId: request.TagId,
            searchText: request.Search
        );

        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var count = await postRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var posts = await postRepo.GetByExpressionAsync(spec, cancellationToken);

        var profileService = provider.GetRequiredService<IProfileService>();

        var items = new List<PostSummaryModel>(posts.Count);

        var localCache = new Dictionary<long, AuthorInfo?>();

        foreach (var post in posts)
        {
            var authorId = post.AuthorId;

            if (!localCache.TryGetValue(authorId, out var author))
            {
                author = await profileService.GetAuthorById(post.AuthorId, cancellationToken);
                localCache[authorId] = author;
            }

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
                    Tags = post
                        .Tags.Select(t => new TagModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug,
                        })
                        .ToList(),
                }
            );
        }

        return new PagedResult<PostSummaryModel>(items, request.PageIndex, request.PageSize, count);
    }
}
