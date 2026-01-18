using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.ContactAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class ContactRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Contact, long>(provider),
        IContactRepository;
