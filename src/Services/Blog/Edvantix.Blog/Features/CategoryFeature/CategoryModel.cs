namespace Edvantix.Blog.Features.CategoryFeature;

/// <summary>
/// Данные категории блога.
/// </summary>
public sealed class CategoryModel
{
    /// <summary>Идентификатор категории.</summary>
    public ulong Id { get; set; }

    /// <summary>Название категории.</summary>
    public string Name { get; set; } = null!;

    /// <summary>URL-слаг категории.</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Описание категории.</summary>
    public string? Description { get; set; }

    /// <summary>Дата и время создания.</summary>
    public DateTime CreatedAt { get; set; }
}
