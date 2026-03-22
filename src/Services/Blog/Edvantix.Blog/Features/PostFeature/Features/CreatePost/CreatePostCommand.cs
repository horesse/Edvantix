using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.CreatePost;

public sealed record CreatePostCommand(
    string Title,
    string Slug,
    string Content,
    string? Summary,
    PostType Type,
    bool IsPremium,
    string? CoverImageUrl,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<Guid> TagIds
) : ICommand<Guid>;

public sealed class CreatePostCommandHandler(ClaimsPrincipal claims, ICategoryRepository categoryRepository, ITagRepository tagRepository, IPostRepository postRepository)
    : ICommandHandler<CreatePostCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreatePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var authorId = claims.GetProfileIdOrError();

        var categories = new List<Category>(request.CategoryIds.Count);

        foreach (var categoryId in request.CategoryIds)
        {
            var category =
                await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            Guard.Against.NotFound(category, categoryId);
            categories.Add(category);
        }

        var tags = new List<Tag>(request.TagIds.Count);

        foreach (var tagId in request.TagIds)
        {
            var tag =
                await tagRepository.GetByIdAsync(tagId, cancellationToken);
            
            Guard.Against.NotFound(tag, tagId);
            tags.Add(tag);
        }

        var post = new Post(
            request.Title,
            request.Slug,
            request.Content,
            request.Summary,
            request.Type,
            authorId,
            request.IsPremium,
            request.CoverImageUrl
        );

        post.SetCategories(categories);
        post.SetTags(tags);

        await postRepository.AddAsync(post, cancellationToken);
        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return post.Id;
    }
}
