using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;

/// <summary>Репозиторий агрегата <see cref="Holiday"/>.</summary>
public interface IHolidayRepository : IRepository<Holiday>
{
    Task<IReadOnlyList<Holiday>> GetByCountryAndYearAsync(
        string countryCode,
        int year,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<Holiday>> ListAsync(
        ISpecification<Holiday> specification,
        CancellationToken cancellationToken = default
    );
}
