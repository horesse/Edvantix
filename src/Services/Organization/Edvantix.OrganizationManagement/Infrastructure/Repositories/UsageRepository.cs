using Edvantix.Chassis.Repository.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;

namespace Edvantix.OrganizationManagement.Infrastructure.Repositories;

public sealed class UsageRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Usage, long>(provider),
        IUsageRepository;
