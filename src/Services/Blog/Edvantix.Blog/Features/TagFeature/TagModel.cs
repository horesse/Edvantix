namespace Edvantix.Blog.Features.TagFeature;

/// <summary>
/// Данные тега блога.
/// </summary>
public sealed class TagModel
{
    /// <summary>Идентификатор тега.</summary>
    public Guid Id { get; set; }

    /// <summary>Название тега.</summary>
    public string Name { get; set; } = null!;

    /// <summary>URL-слаг тега.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Дата и время создания.</summary>
    public DateTime CreatedAt { get; set; }
}
