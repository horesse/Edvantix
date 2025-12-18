using Edvantix.Chassis.Repository.Crud;
using Edvantix.Organization.Domain.AggregatesModel.UsageAggregate;

namespace Edvantix.Organization.Infrastructure.Repositories;

public sealed class UsageRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Usage, long>(provider),
        IUsageRepository;
