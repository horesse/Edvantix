namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

/// <summary>Канал привлечения студента.</summary>
public enum LeadChannel
{
    /// <summary>Онлайн-канал (соцсети, реклама, сайт).</summary>
    Online,

    /// <summary>Офлайн-канал (мероприятия, наружная реклама).</summary>
    Offline,

    /// <summary>Реферал (рекомендация от другого студента или партнёра).</summary>
    Referral,

    /// <summary>Прямое обращение (без посредников).</summary>
    Direct,

    /// <summary>Иной канал.</summary>
    Other,
}
