using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;

/// <summary>
/// Государственный праздник — справочный агрегат.
/// Используется для автоматического пропуска занятий при <c>SkipHolidays = true</c>.
/// </summary>
public sealed class Holiday : Entity, IAggregateRoot
{
    private Holiday() { }

    /// <param name="countryCode">Код страны (ISO 3166-1 alpha-3, напр. <c>RUS</c>).</param>
    /// <param name="date">Дата праздника.</param>
    /// <param name="name">Название праздника.</param>
    /// <param name="isRecurringAnnually">Повторяется ли праздник ежегодно.</param>
    public Holiday(string countryCode, DateOnly date, string name, bool isRecurringAnnually = true)
    {
        Guard.Against.NullOrWhiteSpace(countryCode, nameof(countryCode));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        Id = Guid.CreateVersion7();
        CountryCode = countryCode.Trim().ToUpperInvariant();
        Date = date;
        Name = name.Trim();
        IsRecurringAnnually = isRecurringAnnually;
    }

    /// <summary>Код страны (ISO 3166-1 alpha-3).</summary>
    public string CountryCode { get; private set; } = string.Empty;

    /// <summary>Дата праздника.</summary>
    public DateOnly Date { get; private set; }

    /// <summary>Название праздника.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Ежегодно повторяющийся праздник.</summary>
    public bool IsRecurringAnnually { get; private set; }
}
