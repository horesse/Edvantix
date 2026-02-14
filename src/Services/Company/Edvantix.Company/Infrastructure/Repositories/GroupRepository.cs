using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class GroupRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, Group, long>(provider),
        IGroupRepository;
