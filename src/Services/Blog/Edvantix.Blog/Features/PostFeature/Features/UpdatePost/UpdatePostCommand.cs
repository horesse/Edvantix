using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.UpdatePost;

public sealed record UpdatePostCommand(
    Guid PostId,
    string Title,
    string Slug,
    string Content,
    string? Summary,
    PostType Type,
    bool IsPremium,
    string? CoverImageUrl,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<Guid> TagIds
) : ICommand;

public sealed class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    ITagRepository tagRepository
) : ICommandHandler<UpdatePostCommand>
{
    public async ValueTask<Unit> Handle(
        UpdatePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        var categories = new List<Category>(request.CategoryIds.Count);

        foreach (var categoryId in request.CategoryIds)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            Guard.Against.NotFound(category, categoryId);
            categories.Add(category);
        }

        var tags = new List<Tag>(request.TagIds.Count);

        foreach (var tagId in request.TagIds)
        {
            var tag = await tagRepository.GetByIdAsync(tagId, cancellationToken);

            Guard.Against.NotFound(tag, tagId);
            tags.Add(tag);
        }

        post.UpdateContent(
            request.Title,
            request.Slug,
            request.Content,
            request.Summary,
            request.Type,
            request.IsPremium,
            request.CoverImageUrl
        );

        post.SetCategories(categories);
        post.SetTags(tags);

        await postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
