using Edvantix.Chassis.Repository.Crud;
using Edvantix.Organization.Domain.AggregatesModel.MemberAggregate;

namespace Edvantix.Organization.Infrastructure.Repositories;

public sealed class MemberRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, Member, Guid>(provider),
        IMemberRepository;
