using Edvantix.Chassis.Repository.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.OrganizationManagement.Infrastructure.Repositories;

public sealed class OrganizationRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Organization, long>(provider),
        IOrganizationRepository;

