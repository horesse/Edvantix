using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

/// <summary>
/// Справочный уровень организации (A1, B2 и т.п.) — агрегат, управляемый через UI настроек.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> уникален в рамках организации среди не удалённых.</item>
///   <item><see cref="SortOrder"/> уникален в рамках организации среди не удалённых.</item>
///   <item><see cref="Name"/> — от 1 до 64 символов.</item>
///   <item><see cref="Description"/> — не более 256 символов.</item>
///   <item>Деактивированный уровень нельзя назначить новой группе.</item>
/// </list>
/// </summary>
public sealed class Level() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private const int MaxNameLength = 64;
    private const int MaxDescriptionLength = 256;

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="code">Уникальный код уровня (напр. <c>A1</c>, <c>B2_ADV</c>).</param>
    /// <param name="name">Отображаемое название уровня.</param>
    /// <param name="description">Описание уровня.</param>
    /// <param name="tone">Цветовой тон для UI-бейджа.</param>
    /// <param name="sortOrder">Порядковый номер в выпадающих списках.</param>
    public Level(
        Guid organizationId,
        LevelCode code,
        string name,
        string? description,
        LevelTone tone,
        short sortOrder
    )
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        ArgumentNullException.ThrowIfNull(code);
        ValidateName(name);
        ValidateDescription(description);

        OrganizationId = organizationId;
        Code = code;
        Name = name.Trim();
        Description = description?.Trim();
        Tone = tone;
        SortOrder = sortOrder;
        IsActive = true;
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Уникальный код уровня в рамках организации.</summary>
    public LevelCode Code { get; private set; } = LevelCode.From("NEW");

    /// <summary>Отображаемое название уровня (1–64 символа).</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Описание уровня (не более 256 символов).</summary>
    public string? Description { get; private set; }

    /// <summary>Цветовой тон для UI-бейджа.</summary>
    public LevelTone Tone { get; private set; }

    /// <summary>Порядковый номер в выпадающих списках; уникален в рамках организации.</summary>
    public short SortOrder { get; private set; }

    /// <summary>Активен ли уровень. Деактивированный нельзя выбрать при создании группы.</summary>
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Изменяет код уровня.</summary>
    /// <param name="newCode">Новый код уровня.</param>
    public void ChangeCode(LevelCode newCode)
    {
        ArgumentNullException.ThrowIfNull(newCode);
        Code = newCode;
    }

    /// <summary>Обновляет данные уровня.</summary>
    /// <param name="name">Новое название.</param>
    /// <param name="description">Новое описание.</param>
    /// <param name="tone">Новый цветовой тон.</param>
    /// <param name="sortOrder">Новый порядковый номер.</param>
    public void Update(string name, string? description, LevelTone tone, short sortOrder)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description?.Trim();
        Tone = tone;
        SortOrder = sortOrder;
    }

    /// <summary>Устанавливает новый порядковый номер без изменения других свойств.</summary>
    /// <param name="sortOrder">Новый порядковый номер.</param>
    public void SetSortOrder(short sortOrder) => SortOrder = sortOrder;

    /// <summary>Активирует уровень — он станет доступен для выбора в группах.</summary>
    public void Activate() => IsActive = true;

    /// <summary>Деактивирует уровень — он не будет доступен для новых групп.</summary>
    public void Deactivate() => IsActive = false;

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;

    private static void ValidateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        if (name.Trim().Length > MaxNameLength)
            throw new ArgumentException(
                $"Название уровня не может превышать {MaxNameLength} символов.",
                nameof(name)
            );
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Trim().Length > MaxDescriptionLength)
            throw new ArgumentException(
                $"Описание уровня не может превышать {MaxDescriptionLength} символов.",
                nameof(description)
            );
    }
}
