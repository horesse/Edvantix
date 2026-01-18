using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class OrganizationRepository(IServiceProvider provider)
    : CrudRepository<
        OrganizationContext,
        Domain.AggregatesModel.OrganizationAggregate.Organization,
        long
    >(provider),
        IOrganizationRepository;
