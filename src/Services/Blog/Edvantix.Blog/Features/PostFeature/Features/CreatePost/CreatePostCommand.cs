using Edvantix.Blog.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features.CreatePost;

/// <summary>
/// Команда для создания нового черновика поста.
/// Доступна только администраторам платформы.
/// </summary>
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
) : IRequest<Guid>;

/// <summary>
/// Обработчик команды создания поста.
/// Создаёт черновик поста с указанными категориями и тегами.
/// </summary>
public sealed class CreatePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreatePostCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreatePostCommand request,
        CancellationToken cancellationToken
    )
    {
        var authorId = await provider.GetProfileId(cancellationToken);

        // Проверяем существование категорий
        var categoryRepo = provider.GetRequiredService<ICategoryRepository>();
        var categories = new List<Category>(request.CategoryIds.Count);

        foreach (var categoryId in request.CategoryIds)
        {
            var category =
                await categoryRepo.GetByIdAsync(categoryId, cancellationToken)
                ?? throw new NotFoundException($"Категория с ID {categoryId} не найдена.");

            categories.Add(category);
        }

        // Проверяем существование тегов
        var tagRepo = provider.GetRequiredService<ITagRepository>();
        var tags = new List<Tag>(request.TagIds.Count);

        foreach (var tagId in request.TagIds)
        {
            var tag =
                await tagRepo.GetByIdAsync(tagId, cancellationToken)
                ?? throw new NotFoundException($"Тег с ID {tagId} не найден.");

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

        var postRepo = provider.GetRequiredService<IPostRepository>();
        await postRepo.AddAsync(post, cancellationToken);
        await postRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return post.Id;
    }
}
