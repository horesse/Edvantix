using Edvantix.Chassis.Repository.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;

namespace Edvantix.OrganizationManagement.Infrastructure.Repositories;

public sealed class ContactRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Contact, long>(provider),
        IContactRepository;

