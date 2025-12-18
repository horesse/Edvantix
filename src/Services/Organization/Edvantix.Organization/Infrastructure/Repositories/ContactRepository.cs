using Edvantix.Chassis.Repository.Crud;
using Edvantix.Organization.Domain.AggregatesModel.ContactAggregate;

namespace Edvantix.Organization.Infrastructure.Repositories;

public sealed class ContactRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Contact, long>(provider),
        IContactRepository;
