using Edvantix.Chassis.Repository.Crud;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;

namespace Edvantix.Person.Infrastructure.Repositories;

public sealed class ContactRepository(IServiceProvider provider)
    : SoftDeleteRepository<PersonContext, Contact, long>(provider), IContactRepository;
