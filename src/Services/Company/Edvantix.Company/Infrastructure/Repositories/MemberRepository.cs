using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.MemberAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class MemberRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, Member, Guid>(provider),
        IMemberRepository;
