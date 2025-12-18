using Edvantix.Chassis.Repository.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;

namespace Edvantix.OrganizationManagement.Infrastructure.Repositories;

public sealed class MemberRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, Member, Guid>(provider),
        IMemberRepository;
