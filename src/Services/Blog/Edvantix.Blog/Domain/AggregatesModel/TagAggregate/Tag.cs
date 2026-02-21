using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Blog.Domain.AggregatesModel.TagAggregate;

/// <summary>
/// Тег для дополнительной классификации постов блога.
/// </summary>
public sealed class Tag() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новый тег.
    /// </summary>
    /// <param name="name">Название тега.</param>
    /// <param name="slug">URL-совместимый идентификатор тега (уникальный).</param>
    public Tag(string name, string slug)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));

        Name = name;
        Slug = slug;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Название тега.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>URL-совместимый уникальный идентификатор тега.</summary>
    public string Slug { get; private set; } = null!;

    /// <summary>Дата и время создания тега.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Обновляет название и slug тега.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="slug">Новый slug.</param>
    public void Update(string name, string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));

        Name = name;
        Slug = slug;
    }
}
