using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

/// <summary>
/// Статус студента организации.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> — обязателен, не более 20 символов, уникален в рамках организации.</item>
///   <item>Системные статусы (<see cref="IsSystem"/>) нельзя архивировать и восстанавливать.</item>
/// </list>
/// </summary>
public sealed class StudentStatus : OrganizationScopedLookup
{
    /// <summary>Максимальная длина кода статуса.</summary>
    public const int MaxCodeLength = 20;

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private StudentStatus() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое имя.</param>
    /// <param name="code">Машинный код статуса (уникален per org).</param>
    /// <param name="tone">Визуальный тон для UI.</param>
    /// <param name="isSystem">Признак системной записи (нельзя архивировать).</param>
    /// <param name="order">Порядок сортировки.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public StudentStatus(
        Guid organizationId,
        string name,
        string code,
        StudentStatusTone tone,
        bool isSystem = false,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateCode(code);

        Code = code.Trim();
        Tone = tone;
        IsSystem = isSystem;
    }

    /// <summary>Машинный код статуса (не более 20 символов).</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Визуальный тон для UI-индикации.</summary>
    public StudentStatusTone Tone { get; private set; }

    /// <summary>Системный статус — не может быть архивирован или восстановлен вручную.</summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// Архивирует статус. Для системных статусов выбрасывает <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="by">Идентификатор пользователя.</param>
    /// <exception cref="InvalidOperationException">Системный статус не может быть архивирован.</exception>
    public new void Archive(Guid by)
    {
        if (IsSystem)
            throw new InvalidOperationException("Системный статус не может быть архивирован.");

        base.Archive(by);
    }

    /// <summary>
    /// Восстанавливает статус. Для системных статусов выбрасывает <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="by">Идентификатор пользователя.</param>
    /// <exception cref="InvalidOperationException">Системный статус не может быть восстановлен.</exception>
    public new void Restore(Guid by)
    {
        if (IsSystem)
            throw new InvalidOperationException("Системный статус не может быть восстановлен.");

        base.Restore(by);
    }

    /// <summary>
    /// Обновляет данные статуса.
    /// </summary>
    /// <param name="name">Новое имя.</param>
    /// <param name="code">Новый машинный код.</param>
    /// <param name="tone">Новый визуальный тон.</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя.</param>
    public void Update(string name, string code, StudentStatusTone tone, int order, Guid by)
    {
        Rename(name, by);
        ValidateCode(code);
        Code = code.Trim();
        Tone = tone;
        SetOrder(order, by);
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Код статуса не может быть пустым.", nameof(code));

        if (code.Trim().Length > MaxCodeLength)
            throw new ArgumentException(
                $"Код статуса не может превышать {MaxCodeLength} символов.",
                nameof(code)
            );
    }
}
