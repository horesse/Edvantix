using Edvantix.Blog.Features.CategoryFeature;
using Edvantix.Blog.Features.TagFeature;

namespace Edvantix.Blog.Features.PostFeature.Models;

/// <summary>
/// Полная информация о посте, включая Markdown-содержимое.
/// </summary>
public sealed class PostModel
{
    /// <summary>Идентификатор поста.</summary>
    public ulong Id { get; set; }

    /// <summary>Заголовок поста.</summary>
    public string Title { get; set; } = null!;

    /// <summary>URL-слаг поста.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Содержимое поста в формате Markdown.</summary>
    public string Content { get; set; } = null!;

    /// <summary>Краткое описание поста.</summary>
    public string? Summary { get; set; }

    /// <summary>Тип контента: News или Changelog.</summary>
    public PostType Type { get; set; }

    /// <summary>Текущий статус поста.</summary>
    public PostStatus Status { get; set; }

    /// <summary>Признак премиум-контента.</summary>
    public bool IsPremium { get; set; }

    /// <summary>URL обложки поста.</summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>Количество лайков.</summary>
    public int LikesCount { get; set; }

    /// <summary>Признак того, что текущий авторизованный пользователь поставил лайк.</summary>
    public bool IsLikedByMe { get; set; }

    /// <summary>Дата и время публикации.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Дата и время запланированной публикации.</summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>Дата и время создания поста.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата и время последнего обновления.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Краткая информация об авторе.</summary>
    public AuthorModel? Author { get; set; }

    /// <summary>Категории поста.</summary>
    public IReadOnlyList<CategoryModel> Categories { get; set; } = [];

    /// <summary>Теги поста.</summary>
    public IReadOnlyList<TagModel> Tags { get; set; } = [];
}
