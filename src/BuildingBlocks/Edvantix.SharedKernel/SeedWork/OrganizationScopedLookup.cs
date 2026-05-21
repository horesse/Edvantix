using Edvantix.SharedKernel.Helpers;

namespace Edvantix.SharedKernel.SeedWork;

/// <summary>
/// Базовый класс для справочников, привязанных к организации (тенанту).
/// <para>Используется всеми orgScoped-справочниками настроек (уровни, предметы, статусы и т.п.).</para>
/// <para>Инварианты:</para>
/// <list type="bullet">
///   <item><see cref="OrganizationId"/> — обязателен, не <see cref="Guid.Empty"/>.</item>
///   <item><see cref="Name"/> — от 1 до 120 символов после <c>Trim</c>, не пуст.</item>
///   <item>Архивирование/восстановление — идемпотентны (повторный вызов — no-op).</item>
/// </list>
/// </summary>
public abstract class OrganizationScopedLookup
    : AuditableEntity<Guid>,
        IAggregateRoot,
        ITenanted
{
    /// <summary>Минимальная длина имени после <c>Trim</c>.</summary>
    public const int MinNameLength = 1;

    /// <summary>Максимальная длина имени после <c>Trim</c>.</summary>
    public const int MaxNameLength = 120;

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    protected OrganizationScopedLookup() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое имя записи справочника.</param>
    /// <param name="order">Порядок сортировки в UI (по умолчанию 0).</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись (опционально).</param>
    protected OrganizationScopedLookup(
        Guid organizationId,
        string name,
        int order = 0,
        Guid? createdBy = null
    )
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        ValidateName(name);

        Id = Guid.CreateVersion7();
        OrganizationId = organizationId;
        Name = name.Trim();
        Order = order;
        IsArchived = false;
        CreatedBy = createdBy;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; protected set; }

    /// <summary>Отображаемое имя записи. Длина 1..120 после <c>Trim</c>.</summary>
    public string Name { get; protected set; } = string.Empty;

    /// <summary>Признак того, что запись архивирована (логически удалена для UI).</summary>
    public bool IsArchived { get; protected set; }

    /// <summary>Порядок сортировки в UI. Меньше значение — выше в списке.</summary>
    public int Order { get; protected set; }

    /// <summary>Идентификатор пользователя, создавшего запись.</summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>Идентификатор пользователя, последним изменившего запись.</summary>
    public Guid? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Архивирует запись справочника. Повторный вызов — no-op.
    /// </summary>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Archive(Guid by)
    {
        if (IsArchived)
            return;

        IsArchived = true;
        Touch(by);
    }

    /// <summary>
    /// Восстанавливает архивную запись. Повторный вызов на активной записи — no-op.
    /// </summary>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Restore(Guid by)
    {
        if (!IsArchived)
            return;

        IsArchived = false;
        Touch(by);
    }

    /// <summary>
    /// Переименовывает запись. Валидирует имя и обновляет audit-поля.
    /// </summary>
    /// <param name="name">Новое имя справочной записи.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    /// <exception cref="ArgumentException">Имя пустое или превышает <see cref="MaxNameLength"/>.</exception>
    public void Rename(string name, Guid by)
    {
        ValidateName(name);
        var trimmed = name.Trim();
        if (string.Equals(trimmed, Name, StringComparison.Ordinal))
            return;

        Name = trimmed;
        Touch(by);
    }

    /// <summary>Изменяет порядок сортировки записи.</summary>
    /// <param name="order">Новое значение порядка.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void SetOrder(int order, Guid by)
    {
        if (order == Order)
            return;

        Order = order;
        Touch(by);
    }

    private void Touch(Guid by)
    {
        LastModifiedAt = DateTimeHelper.UtcNow();
        LastModifiedBy = by == Guid.Empty ? null : by;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Имя записи справочника не может быть пустым.",
                nameof(name)
            );

        var length = name.Trim().Length;
        if (length is < MinNameLength or > MaxNameLength)
            throw new ArgumentException(
                $"Имя записи справочника должно быть от {MinNameLength} до {MaxNameLength} символов.",
                nameof(name)
            );
    }
}
