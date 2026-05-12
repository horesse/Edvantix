namespace Edvantix.Schedule.Features.Holidays;

public sealed record HolidayDto(
    Guid Id,
    string CountryCode,
    DateOnly Date,
    string Name,
    bool IsRecurringAnnually
);
