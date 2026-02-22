namespace Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;

/// <summary>
/// Категория постов блога. Создаётся и управляется администраторами платформы.
/// </summary>
public sealed class Category() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую категорию блога.
    /// </summary>
    /// <param name="name">Название категории.</param>
    /// <param name="slug">URL-совместимый идентификатор категории (уникальный).</param>
    /// <param name="description">Описание категории.</param>
    public Category(string name, string slug, string? description = null)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        Name = name;
        Slug = slug;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Название категории.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>URL-совместимый уникальный идентификатор категории.</summary>
    public string Slug { get; private set; } = null!;

    /// <summary>Описание категории.</summary>
    public string? Description { get; private set; }

    /// <summary>Дата и время создания категории.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Обновляет название и описание категории.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="slug">Новый slug.</param>
    /// <param name="description">Новое описание.</param>
    public void Update(string name, string slug, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        Name = name;
        Slug = slug;
        Description = description;
    }
}
