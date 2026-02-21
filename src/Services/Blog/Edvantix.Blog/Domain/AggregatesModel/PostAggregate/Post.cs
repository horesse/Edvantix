using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Events;
using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Пост блога платформы Edvantix.
/// Является агрегатным корнем для управления контентом, лайками, категориями и тегами.
/// </summary>
public sealed class Post() : Entity, IAggregateRoot
{
    private readonly List<PostLike> _likes = [];
    private readonly List<Category> _categories = [];
    private readonly List<Tag> _tags = [];

    /// <summary>
    /// Создаёт новый черновик поста.
    /// </summary>
    /// <param name="title">Заголовок поста.</param>
    /// <param name="slug">URL-совместимый идентификатор поста (уникальный).</param>
    /// <param name="content">Содержимое поста в формате Markdown.</param>
    /// <param name="summary">Краткое описание поста для превью.</param>
    /// <param name="type">Тип контента: новость или changelog.</param>
    /// <param name="authorId">Идентификатор профиля автора (администратора).</param>
    /// <param name="isPremium">Признак премиум-контента, доступного только подписчикам.</param>
    /// <param name="coverImageUrl">URL обложки поста.</param>
    public Post(
        string title,
        string slug,
        string content,
        string? summary,
        PostType type,
        ulong authorId,
        bool isPremium = false,
        string? coverImageUrl = null
    )
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        if (authorId <= 0)
            throw new ArgumentException("Некорректный идентификатор автора.", nameof(authorId));

        Title = title;
        Slug = slug;
        Content = content;
        Summary = summary;
        Type = type;
        AuthorId = authorId;
        IsPremium = isPremium;
        CoverImageUrl = coverImageUrl;
        Status = PostStatus.Draft;
        LikesCount = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Заголовок поста.</summary>
    public string Title { get; private set; } = null!;

    /// <summary>URL-совместимый уникальный идентификатор поста.</summary>
    public string Slug { get; private set; } = null!;

    /// <summary>Содержимое поста в формате Markdown.</summary>
    public string Content { get; private set; } = null!;

    /// <summary>Краткое описание поста для превью и SEO.</summary>
    public string? Summary { get; private set; }

    /// <summary>Тип контента: News или Changelog.</summary>
    public PostType Type { get; private set; }

    /// <summary>Текущий статус поста в жизненном цикле.</summary>
    public PostStatus Status { get; private set; }

    /// <summary>Признак премиум-контента, доступного только для платных подписчиков.</summary>
    public bool IsPremium { get; private set; }

    /// <summary>Идентификатор профиля автора поста.</summary>
    public ulong AuthorId { get; private set; }

    /// <summary>Дата и время публикации поста.</summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>Дата и время запланированной публикации.</summary>
    public DateTime? ScheduledAt { get; private set; }

    /// <summary>URL обложки поста.</summary>
    public string? CoverImageUrl { get; private set; }

    /// <summary>Денормализованный счётчик лайков для быстрого отображения.</summary>
    public int LikesCount { get; private set; }

    /// <summary>Дата и время создания поста.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата и время последнего обновления поста.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Лайки поста.</summary>
    public IReadOnlyCollection<PostLike> Likes => _likes.AsReadOnly();

    /// <summary>Категории, к которым относится пост.</summary>
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

    /// <summary>Теги, присвоенные посту.</summary>
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Обновляет основное содержимое поста.
    /// Доступно только для постов в статусе Draft или Scheduled.
    /// </summary>
    public void UpdateContent(
        string title,
        string slug,
        string content,
        string? summary,
        PostType type,
        bool isPremium,
        string? coverImageUrl
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        if (Status == PostStatus.Archived)
            throw new InvalidOperationException("Нельзя редактировать архивный пост.");

        Title = title;
        Slug = slug;
        Content = content;
        Summary = summary;
        Type = type;
        IsPremium = isPremium;
        CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Немедленно публикует пост, переводя его в статус Published.
    /// </summary>
    public void Publish()
    {
        if (Status == PostStatus.Published)
            throw new InvalidOperationException("Пост уже опубликован.");

        if (Status == PostStatus.Archived)
            throw new InvalidOperationException("Нельзя опубликовать архивный пост.");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        ScheduledAt = null;
        UpdatedAt = DateTime.UtcNow;

        RegisterDomainEvent(new PostPublishedEvent(Id, Title, Slug, Type, IsPremium, AuthorId));
    }

    /// <summary>
    /// Планирует публикацию поста на указанное время.
    /// </summary>
    /// <param name="scheduledAt">Дата и время публикации (должна быть в будущем).</param>
    public void Schedule(DateTime scheduledAt)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException(
                "Дата публикации должна быть в будущем.",
                nameof(scheduledAt)
            );

        if (Status == PostStatus.Published)
            throw new InvalidOperationException("Пост уже опубликован.");

        if (Status == PostStatus.Archived)
            throw new InvalidOperationException("Нельзя запланировать архивный пост.");

        Status = PostStatus.Scheduled;
        ScheduledAt = scheduledAt;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Переводит пост в архив, скрывая его из публичного списка.
    /// </summary>
    public void Archive()
    {
        if (Status == PostStatus.Archived)
            throw new InvalidOperationException("Пост уже находится в архиве.");

        Status = PostStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Переводит пост обратно в черновик для редактирования.
    /// </summary>
    public void ReturnToDraft()
    {
        if (Status == PostStatus.Draft)
            throw new InvalidOperationException("Пост уже является черновиком.");

        Status = PostStatus.Draft;
        PublishedAt = null;
        ScheduledAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Увеличивает счётчик лайков поста на единицу.
    /// Вызывается после успешного создания записи PostLike в репозитории.
    /// </summary>
    public void IncrementLikesCount()
    {
        LikesCount++;
    }

    /// <summary>
    /// Уменьшает счётчик лайков поста на единицу (минимум 0).
    /// Вызывается после успешного удаления записи PostLike из репозитория.
    /// </summary>
    public void DecrementLikesCount()
    {
        LikesCount = Math.Max(0, LikesCount - 1);
    }

    /// <summary>
    /// Устанавливает новый набор категорий, заменяя текущий.
    /// </summary>
    /// <param name="categories">Новый набор категорий.</param>
    public void SetCategories(IEnumerable<Category> categories)
    {
        _categories.Clear();

        foreach (var category in categories)
        {
            _categories.Add(category);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Устанавливает новый набор тегов, заменяя текущий.
    /// </summary>
    /// <param name="tags">Новый набор тегов.</param>
    public void SetTags(IEnumerable<Tag> tags)
    {
        _tags.Clear();

        foreach (var tag in tags)
        {
            _tags.Add(tag);
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
