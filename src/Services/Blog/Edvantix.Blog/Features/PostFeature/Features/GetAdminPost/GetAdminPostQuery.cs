using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Specifications;
using Edvantix.Blog.Features.CategoryFeature.Models;
using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Features.TagFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPost;

/// <summary>
/// Административный запрос для получения полного содержимого поста по ID.
/// В отличие от публичного GetPostBySlugQuery возвращает пост любого статуса.
/// </summary>
public sealed record GetAdminPostQuery(long PostId) : IRequest<PostModel>;

/// <summary>
/// Обработчик административного запроса на получение поста.
/// </summary>
public sealed class GetAdminPostQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetAdminPostQuery, PostModel>
{
    public async Task<PostModel> Handle(
        GetAdminPostQuery request,
        CancellationToken cancellationToken
    )
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        var profileService = provider.GetRequiredService<IProfileService>();
        var author = await profileService.GetAuthorById(post.AuthorId, cancellationToken);

        var currentUserId = await provider.TryGetProfileId(cancellationToken);
        var isLikedByMe = false;

        if (currentUserId.HasValue)
        {
            using var likeRepo = provider.GetRequiredService<IPostLikeRepository>();
            var likeSpec = new PostLikeSpecification(postId: post.Id, userId: currentUserId.Value);
            var existingLike = await likeRepo.GetFirstByExpressionAsync(likeSpec, cancellationToken);
            isLikedByMe = existingLike is not null;
        }

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
            IsLikedByMe = isLikedByMe,
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
