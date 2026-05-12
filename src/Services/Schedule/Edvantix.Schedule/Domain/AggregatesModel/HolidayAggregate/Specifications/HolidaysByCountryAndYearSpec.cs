namespace Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate.Specifications;

internal sealed class HolidaysByCountryAndYearSpec : Specification<Holiday>
{
    public HolidaysByCountryAndYearSpec(string countryCode, int year)
    {
        var upper = countryCode.ToUpperInvariant();
        Query
            .Where(h => h.CountryCode == upper && (h.Date.Year == year || h.IsRecurringAnnually))
            .OrderBy(h => h.Date);
    }
}
