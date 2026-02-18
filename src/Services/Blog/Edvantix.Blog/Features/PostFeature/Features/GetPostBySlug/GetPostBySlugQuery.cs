using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Blog.Features.CategoryFeature.Models;
using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Features.TagFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostBySlug;

/// <summary>
/// Запрос для получения полного содержимого поста по slug.
/// </summary>
public sealed record GetPostBySlugQuery(string Slug) : IRequest<PostModel>;

/// <summary>
/// Обработчик запроса на получение поста по slug.
/// Для премиум-постов возвращает данные независимо от статуса подписки на уровне API
/// (проверка осуществляется клиентом или через middleware в будущем).
/// </summary>
public sealed class GetPostBySlugQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetPostBySlugQuery, PostModel>
{
    public async Task<PostModel> Handle(
        GetPostBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetOrDefaultAsync(
                p => p.Slug == request.Slug && p.Status == PostStatus.Published,
                cancellationToken
            ) ?? throw new NotFoundException($"Пост со slug '{request.Slug}' не найден.");

        var profileService = provider.GetRequiredService<IProfileService>();
        var author = await profileService.GetAuthorById(post.AuthorId, cancellationToken);

        return new PostModel
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Content = post.Content,
            Summary = post.Summary,
            Type = post.Type,
            Status = post.Status,
            IsPremium = post.IsPremium,
            CoverImageUrl = post.CoverImageUrl,
            LikesCount = post.LikesCount,
            PublishedAt = post.PublishedAt,
            ScheduledAt = post.ScheduledAt,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Author = author is null
                ? null
                : new AuthorModel { Id = author.Id, FullName = author.FullName },
            Categories = post
                .Categories.Select(c => new CategoryModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
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
        };
    }
}
