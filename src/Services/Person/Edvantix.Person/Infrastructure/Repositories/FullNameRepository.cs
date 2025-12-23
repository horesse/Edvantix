using Edvantix.Chassis.Repository.Crud;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;

namespace Edvantix.Person.Infrastructure.Repositories;

public sealed class FullNameRepository(IServiceProvider provider)
    : SoftDeleteRepository<PersonContext, FullName, long>(provider), IFullNameRepository;
