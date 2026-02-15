using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class GroupMemberRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, GroupMember, Guid>(provider),
        IGroupMemberRepository;
