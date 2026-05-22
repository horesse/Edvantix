using System.Text.RegularExpressions;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Groups.Domain.LessonTypeAggregate;

/// <summary>
/// Тип занятия организации — справочный агрегат (урок, консультация, тест, мастер-класс).
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> уникален в рамках организации среди не архивных записей.</item>
///   <item><see cref="DefaultDurationMinutes"/> — от 5 до 600 минут.</item>
///   <item><see cref="Color"/> — шестизначный HEX-цвет формата <c>#RRGGBB</c>.</item>
///   <item><see cref="Icon"/> — имя иконки из kit (не более 40 символов), опционально.</item>
/// </list>
/// </summary>
public sealed class LessonType : OrganizationScopedLookup
{
    /// <summary>Максимальная длина кода типа занятия.</summary>
    public const int MaxCodeLength = 20;

    /// <summary>Максимальная длина имени иконки.</summary>
    public const int MaxIconLength = 40;

    /// <summary>Минимальная длительность занятия в минутах.</summary>
    public const int MinDurationMinutes = 5;

    /// <summary>Максимальная длительность занятия в минутах.</summary>
    public const int MaxDurationMinutes = 600;

    // Паттерн HEX-цвета: #RRGGBB
    private static readonly Regex HexColorRegex = new(
        @"^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    // Паттерн кода: только заглавные латинские буквы, цифры, дефисы и подчёркивания
    private static readonly Regex CodeRegex = new(
        @"^[A-Z0-9_-]{1,20}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private LessonType() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое имя типа занятия.</param>
    /// <param name="code">Уникальный код в рамках организации (напр. <c>LESSON</c>).</param>
    /// <param name="defaultDurationMinutes">Длительность занятия по умолчанию (5–600 минут).</param>
    /// <param name="color">HEX-цвет для UI (#RRGGBB).</param>
    /// <param name="icon">Имя иконки из kit (опционально, не более 40 символов).</param>
    /// <param name="order">Порядок сортировки в UI (по умолчанию 0).</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public LessonType(
        Guid organizationId,
        string name,
        string code,
        int defaultDurationMinutes,
        string color,
        string? icon,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateCode(code);
        ValidateDuration(defaultDurationMinutes);
        ValidateColor(color);
        ValidateIcon(icon);

        Code = NormalizeCode(code);
        DefaultDurationMinutes = defaultDurationMinutes;
        Color = color.ToUpperInvariant();
        Icon = icon?.Trim();
    }

    /// <summary>Уникальный код типа занятия в рамках организации (только заглавные, цифры, -, _).</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Длительность занятия по умолчанию в минутах (5–600).</summary>
    public int DefaultDurationMinutes { get; private set; }

    /// <summary>Цвет для UI-отображения в формате #RRGGBB.</summary>
    public string Color { get; private set; } = string.Empty;

    /// <summary>Имя иконки из kit (опционально).</summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Обновляет все изменяемые поля типа занятия.
    /// </summary>
    /// <param name="name">Новое имя.</param>
    /// <param name="code">Новый код.</param>
    /// <param name="defaultDurationMinutes">Новая длительность по умолчанию.</param>
    /// <param name="color">Новый цвет (#RRGGBB).</param>
    /// <param name="icon">Новое имя иконки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(
        string name,
        string code,
        int defaultDurationMinutes,
        string color,
        string? icon,
        Guid by
    )
    {
        ValidateCode(code);
        ValidateDuration(defaultDurationMinutes);
        ValidateColor(color);
        ValidateIcon(icon);

        Rename(name, by);

        Code = NormalizeCode(code);
        DefaultDurationMinutes = defaultDurationMinutes;
        Color = color.ToUpperInvariant();
        Icon = icon?.Trim();

        // Обновляем аудит-поля (даже если имя не изменилось)
        LastModifiedAt = DateTimeHelper.UtcNow();
        LastModifiedBy = by == Guid.Empty ? null : by;
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Код типа занятия не может быть пустым.", nameof(code));

        if (!CodeRegex.IsMatch(code.Trim().ToUpperInvariant()))
            throw new ArgumentException(
                $"Код типа занятия должен содержать только заглавные латинские буквы, цифры, дефисы и подчёркивания, и не превышать {MaxCodeLength} символов.",
                nameof(code)
            );
    }

    private static void ValidateDuration(int minutes)
    {
        if (minutes is < MinDurationMinutes or > MaxDurationMinutes)
            throw new ArgumentException(
                $"Длительность занятия должна быть от {MinDurationMinutes} до {MaxDurationMinutes} минут.",
                nameof(minutes)
            );
    }

    private static void ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Цвет типа занятия не может быть пустым.", nameof(color));

        if (!HexColorRegex.IsMatch(color))
            throw new ArgumentException(
                "Цвет должен быть в формате HEX (#RRGGBB).",
                nameof(color)
            );
    }

    private static void ValidateIcon(string? icon)
    {
        if (icon is not null && icon.Trim().Length > MaxIconLength)
            throw new ArgumentException(
                $"Имя иконки не может превышать {MaxIconLength} символов.",
                nameof(icon)
            );
    }
}
