using Edvantix.Blog.Features.CategoryFeature;
using Edvantix.Blog.Features.TagFeature;

namespace Edvantix.Blog.Features.PostFeature.Models;

/// <summary>
/// Краткая информация о посте для списков (без полного содержимого).
/// </summary>
public sealed class PostSummaryModel
{
    /// <summary>Идентификатор поста.</summary>
    public ulong Id { get; set; }

    /// <summary>Заголовок поста.</summary>
    public string Title { get; set; } = null!;

    /// <summary>URL-слаг для прямой ссылки на пост.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Краткое описание поста.</summary>
    public string? Summary { get; set; }

    /// <summary>Статус поста.</summary>
    public PostStatus Status { get; set; }

    /// <summary>Тип контента: News или Changelog.</summary>
    public PostType Type { get; set; }

    /// <summary>Признак премиум-контента.</summary>
    public bool IsPremium { get; set; }

    /// <summary>URL обложки поста.</summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>Количество лайков.</summary>
    public int LikesCount { get; set; }

    /// <summary>Дата и время публикации.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Дата и время запланированной публикации.</summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>Краткая информация об авторе.</summary>
    public AuthorModel? Author { get; set; }

    /// <summary>Категории поста.</summary>
    public IReadOnlyList<CategoryModel> Categories { get; set; } = [];

    /// <summary>Теги поста.</summary>
    public IReadOnlyList<TagModel> Tags { get; set; } = [];
}
