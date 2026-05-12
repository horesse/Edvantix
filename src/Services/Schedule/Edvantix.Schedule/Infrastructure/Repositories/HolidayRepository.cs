using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;

namespace Edvantix.Schedule.Infrastructure.Repositories;

internal sealed class HolidayRepository(ScheduleDbContext context) : IHolidayRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<IReadOnlyList<Holiday>> GetByCountryAndYearAsync(
        string countryCode,
        int year,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Holidays.AsNoTracking()
            .Where(h =>
                h.CountryCode == countryCode.ToUpperInvariant()
                && (h.Date.Year == year || h.IsRecurringAnnually)
            )
            .OrderBy(h => h.Date)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Holiday>> ListAsync(
        ISpecification<Holiday> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Holidays.AsNoTracking(), specification)
            .ToListAsync(cancellationToken);
}
