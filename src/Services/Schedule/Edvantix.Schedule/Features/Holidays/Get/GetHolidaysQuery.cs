using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Edvantix.Schedule.Features.Holidays.Specifications;

namespace Edvantix.Schedule.Features.Holidays.Get;

public sealed record GetHolidaysQuery(string CountryCode, int Year)
    : IQuery<IReadOnlyList<HolidayDto>>;

internal sealed class GetHolidaysQueryHandler(
    IHolidayRepository repository
) : IQueryHandler<GetHolidaysQuery, IReadOnlyList<HolidayDto>>
{
    public async ValueTask<IReadOnlyList<HolidayDto>> Handle(
        GetHolidaysQuery query,
        CancellationToken cancellationToken
    )
    {
        var spec = new HolidaysByCountryAndYearSpec(query.CountryCode, query.Year);
        var holidays = await repository.ListAsync(spec, cancellationToken);

        return holidays
            .Select(h => new HolidayDto(h.Id, h.CountryCode, h.Date, h.Name, h.IsRecurringAnnually))
            .ToList();
    }
}
