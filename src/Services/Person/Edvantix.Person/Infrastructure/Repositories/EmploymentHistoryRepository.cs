using Edvantix.Chassis.Repository.Crud;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;

namespace Edvantix.Person.Infrastructure.Repositories;

public sealed class EmploymentHistoryRepository(IServiceProvider provider)
    : SoftDeleteRepository<PersonContext, EmploymentHistory, long>(provider), IEmploymentHistoryRepository;
