using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

/// <summary>
/// Источник привлечения студента — запись справочника «Источники привлечения».
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Channel"/> — канал для группировки в отчётах.</item>
///   <item><see cref="UtmTag"/> — до 60 символов после <c>Trim</c>; <c>null</c> допустим.</item>
///   <item>Уникальность <see cref="OrganizationScopedLookup.Name"/> в рамках организации среди не архивных записей.</item>
/// </list>
/// </summary>
public sealed class LeadSource : OrganizationScopedLookup
{
    /// <summary>Максимальная длина UTM-метки.</summary>
    public const int MaxUtmTagLength = 60;

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private LeadSource() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое название источника.</param>
    /// <param name="channel">Канал привлечения для группировки в отчётах.</param>
    /// <param name="utmTag">UTM-метка для атрибуции (до 60 символов); <c>null</c> — не указана.</param>
    /// <param name="order">Порядок сортировки.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public LeadSource(
        Guid organizationId,
        string name,
        LeadChannel channel,
        string? utmTag,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateUtmTag(utmTag);

        Channel = channel;
        UtmTag = utmTag?.Trim();
    }

    /// <summary>Канал привлечения (для группировки в отчётах).</summary>
    public LeadChannel Channel { get; private set; }

    /// <summary>UTM-метка для атрибуции (до 60 символов). Может быть <c>null</c>.</summary>
    public string? UtmTag { get; private set; }

    /// <summary>
    /// Обновляет данные источника привлечения.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="channel">Новый канал привлечения.</param>
    /// <param name="utmTag">Новая UTM-метка (до 60 символов).</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(string name, LeadChannel channel, string? utmTag, int order, Guid by)
    {
        Rename(name, by);
        ValidateUtmTag(utmTag);

        Channel = channel;
        UtmTag = utmTag?.Trim();

        SetOrder(order, by);
    }

    private static void ValidateUtmTag(string? utmTag)
    {
        if (utmTag is not null && utmTag.Trim().Length > MaxUtmTagLength)
            throw new ArgumentException(
                $"UTM-метка не может превышать {MaxUtmTagLength} символов.",
                nameof(utmTag)
            );
    }
}
