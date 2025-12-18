using Edvantix.Chassis.Repository.Crud;
using Edvantix.Organization.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.Organization.Infrastructure.Repositories;

public sealed class OrganizationRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Domain.AggregatesModel.OrganizationAggregate.Organization, long>(provider),
        IOrganizationRepository;
