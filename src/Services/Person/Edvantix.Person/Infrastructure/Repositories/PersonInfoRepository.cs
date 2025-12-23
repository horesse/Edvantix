using Edvantix.Chassis.Repository.Crud;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;

namespace Edvantix.Person.Infrastructure.Repositories;

public sealed class PersonInfoRepository(IServiceProvider provider)
    : CrudRepository<PersonContext, PersonInfo, long>(provider), IPersonInfoRepository;
