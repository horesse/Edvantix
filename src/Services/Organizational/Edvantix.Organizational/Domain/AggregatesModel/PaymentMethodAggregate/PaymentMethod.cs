using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;

/// <summary>
/// Способ оплаты организации — запись справочника «Способы оплаты».
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> — обязателен, не более 20 символов, уникален в рамках организации.</item>
///   <item><see cref="IsCashless"/> — признак безналичного расчёта (для отчётности).</item>
///   <item><see cref="RequiresContract"/> — признак необходимости договора (напр., рассрочка).</item>
///   <item>Уникальность <see cref="OrganizationScopedLookup.Name"/> среди не архивных записей организации.</item>
/// </list>
/// </summary>
public sealed class PaymentMethod : OrganizationScopedLookup
{
    /// <summary>Максимальная длина кода способа оплаты.</summary>
    public const int MaxCodeLength = 20;

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private PaymentMethod() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое название способа оплаты.</param>
    /// <param name="code">Машинный код (уникален per org, до 20 символов).</param>
    /// <param name="isCashless">Признак безналичного расчёта.</param>
    /// <param name="requiresContract">Признак необходимости договора.</param>
    /// <param name="order">Порядок сортировки.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public PaymentMethod(
        Guid organizationId,
        string name,
        string code,
        bool isCashless,
        bool requiresContract,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateCode(code);

        Code = code.Trim();
        IsCashless = isCashless;
        RequiresContract = requiresContract;
    }

    /// <summary>Машинный код способа оплаты (уникален per org, до 20 символов).</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Признак безналичного расчёта (для отчётности).</summary>
    public bool IsCashless { get; private set; }

    /// <summary>Признак необходимости договора (например, рассрочка).</summary>
    public bool RequiresContract { get; private set; }

    /// <summary>
    /// Обновляет данные способа оплаты.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="code">Новый машинный код.</param>
    /// <param name="isCashless">Новый признак безналичного расчёта.</param>
    /// <param name="requiresContract">Новый признак необходимости договора.</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(
        string name,
        string code,
        bool isCashless,
        bool requiresContract,
        int order,
        Guid by
    )
    {
        Rename(name, by);
        ValidateCode(code);

        Code = code.Trim();
        IsCashless = isCashless;
        RequiresContract = requiresContract;

        SetOrder(order, by);
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Код способа оплаты не может быть пустым.", nameof(code));

        if (code.Trim().Length > MaxCodeLength)
            throw new ArgumentException(
                $"Код способа оплаты не может превышать {MaxCodeLength} символов.",
                nameof(code)
            );
    }
}
