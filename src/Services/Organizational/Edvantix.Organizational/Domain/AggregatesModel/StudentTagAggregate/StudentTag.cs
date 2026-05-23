using System.Text.RegularExpressions;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;

/// <summary>
/// Тег студента — свободная метка для сегментации студентов в рамках организации.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="OrganizationScopedLookup.Name"/> — от 1 до 40 символов после <c>Trim</c>.</item>
///   <item><see cref="Color"/> — HEX-цвет в формате <c>#RRGGBB</c> (7 символов, обязателен).</item>
///   <item>Уникальность <see cref="OrganizationScopedLookup.Name"/> в рамках организации среди не архивных записей.</item>
/// </list>
/// </summary>
public sealed class StudentTag : OrganizationScopedLookup
{
    /// <summary>Максимальная длина имени тега после <c>Trim</c>.</summary>
    public new const int MaxNameLength = 40;

    /// <summary>Регулярное выражение для проверки HEX-цвета (<c>#RRGGBB</c>).</summary>
    private static readonly Regex HexColorRegex = new(
        @"^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private StudentTag() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое название тега (1–40 символов).</param>
    /// <param name="color">Цвет метки в формате HEX <c>#RRGGBB</c>.</param>
    /// <param name="order">Порядок сортировки.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public StudentTag(
        Guid organizationId,
        string name,
        string color,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateTagName(name);
        ValidateColor(color);

        Color = color.Trim().ToUpperInvariant();
    }

    /// <summary>Цвет метки в формате HEX <c>#RRGGBB</c> (например, <c>#FF5733</c>).</summary>
    public string Color { get; private set; } = "#000000";

    /// <summary>
    /// Обновляет данные тега.
    /// </summary>
    /// <param name="name">Новое название (1–40 символов).</param>
    /// <param name="color">Новый цвет в формате HEX <c>#RRGGBB</c>.</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(string name, string color, int order, Guid by)
    {
        ValidateTagName(name);
        Rename(name, by);
        ValidateColor(color);

        var upper = color.Trim().ToUpperInvariant();
        if (!string.Equals(upper, Color, StringComparison.Ordinal))
        {
            Color = upper;
            Touch(by);
        }

        SetOrder(order, by);
    }

    private static void ValidateTagName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length > MaxNameLength)
            throw new ArgumentException(
                $"Название тега не может превышать {MaxNameLength} символов.",
                nameof(name)
            );
    }

    private static void ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || !HexColorRegex.IsMatch(color.Trim()))
            throw new ArgumentException(
                "Цвет тега должен быть в формате HEX #RRGGBB (например, #FF5733).",
                nameof(color)
            );
    }
}
