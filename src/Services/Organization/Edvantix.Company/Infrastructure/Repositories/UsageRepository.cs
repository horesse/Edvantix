using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.UsageAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class UsageRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Usage, long>(provider),
        IUsageRepository;
