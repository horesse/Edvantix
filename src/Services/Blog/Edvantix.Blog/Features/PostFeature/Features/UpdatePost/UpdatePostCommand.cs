namespace Edvantix.Blog.Features.PostFeature.Features.UpdatePost;

/// <summary>
/// Команда для обновления содержимого поста.
/// Доступна только администраторам платформы.
/// </summary>
public sealed record UpdatePostCommand(
    long PostId,
    string Title,
    string Slug,
    string Content,
    string? Summary,
    PostType Type,
    bool IsPremium,
    string? CoverImageUrl,
    IReadOnlyList<long> CategoryIds,
    IReadOnlyList<long> TagIds
) : IRequest;

/// <summary>
/// Обработчик команды обновления поста.
/// </summary>
public sealed class UpdatePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdatePostCommand>
{
    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        // Проверяем существование категорий
        using var categoryRepo = provider.GetRequiredService<ICategoryRepository>();
        var categories = new List<Category>(request.CategoryIds.Count);

        foreach (var categoryId in request.CategoryIds)
        {
            var category =
                await categoryRepo.GetByIdAsync(categoryId, cancellationToken)
                ?? throw new NotFoundException($"Категория с ID {categoryId} не найдена.");

            categories.Add(category);
        }

        // Проверяем существование тегов
        using var tagRepo = provider.GetRequiredService<ITagRepository>();
        var tags = new List<Tag>(request.TagIds.Count);

        foreach (var tagId in request.TagIds)
        {
            var tag =
                await tagRepo.GetByIdAsync(tagId, cancellationToken)
                ?? throw new NotFoundException($"Тег с ID {tagId} не найден.");

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

        await postRepo.UpdateAsync(post, cancellationToken);
        await postRepo.SaveEntitiesAsync(cancellationToken);
    }
}
