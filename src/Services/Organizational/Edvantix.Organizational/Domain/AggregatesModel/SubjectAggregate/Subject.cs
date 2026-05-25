using System.Text.RegularExpressions;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;

/// <summary>
/// Учебный предмет организации — агрегат справочника, управляемого через UI настроек.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> уникален в рамках организации среди не архивных.</item>
///   <item><see cref="Name"/> уникален в рамках организации среди не архивных (наследуется).</item>
///   <item><see cref="Color"/> — строка формата <c>#RRGGBB</c> (HEX, заглавные буквы).</item>
///   <item><see cref="Description"/> — не более 500 символов.</item>
/// </list>
/// </summary>
public sealed class Subject : OrganizationScopedLookup
{
    /// <summary>Цвет предмета по умолчанию (индиго).</summary>
    public const string DefaultColor = "#6366F1";

    private const int MaxDescriptionLength = 500;

    /// <summary>Регулярное выражение допустимого формата цвета в формате #RRGGBB.</summary>
    private static readonly Regex HexColorPattern = new(
        @"^#[0-9A-F]{6}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private Subject() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое название предмета.</param>
    /// <param name="code">Уникальный код предмета (напр. <c>MATH</c>, <c>ENG101</c>).</param>
    /// <param name="color">Цвет предмета в формате <c>#RRGGBB</c>. По умолчанию <see cref="DefaultColor"/>.</param>
    /// <param name="description">Описание предмета (опционально).</param>
    /// <param name="order">Порядок сортировки в UI.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public Subject(
        Guid organizationId,
        string name,
        SubjectCode code,
        string color,
        string? description,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ArgumentNullException.ThrowIfNull(code);
        ValidateColor(color);
        ValidateDescription(description);

        Code = code;
        Color = NormalizeColor(color);
        Description = description?.Trim();
    }

    /// <summary>Уникальный код предмета в рамках организации.</summary>
    public SubjectCode Code { get; private set; } = SubjectCode.From("NEW");

    /// <summary>Цвет предмета в формате <c>#RRGGBB</c>.</summary>
    public string Color { get; private set; } = DefaultColor;

    /// <summary>Описание предмета (не более 500 символов).</summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Обновляет поля предмета. Вызывает <see cref="OrganizationScopedLookup.Rename"/> и <see cref="OrganizationScopedLookup.SetOrder"/> для унаследованных полей.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="code">Новый код.</param>
    /// <param name="color">Новый цвет в формате <c>#RRGGBB</c>.</param>
    /// <param name="description">Новое описание.</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(
        string name,
        SubjectCode code,
        string color,
        string? description,
        int order,
        Guid by
    )
    {
        ArgumentNullException.ThrowIfNull(code);
        ValidateColor(color);
        ValidateDescription(description);

        Code = code;
        Color = NormalizeColor(color);
        Description = description?.Trim();

        // Вызывает Touch(by) при изменении name или order
        Rename(name, by);
        SetOrder(order, by);

        // Гарантируем обновление audit-полей даже если name и order не изменились
        Touch(by);
    }

    private static void ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Цвет предмета не может быть пустым.", nameof(color));

        var normalized = NormalizeColor(color);
        if (!HexColorPattern.IsMatch(normalized))
            throw new ArgumentException(
                "Цвет предмета должен быть в формате #RRGGBB (напр. #6366F1).",
                nameof(color)
            );
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Trim().Length > MaxDescriptionLength)
            throw new ArgumentException(
                $"Описание предмета не может превышать {MaxDescriptionLength} символов.",
                nameof(description)
            );
    }

    private static string NormalizeColor(string color) => color.Trim().ToUpperInvariant();
}
