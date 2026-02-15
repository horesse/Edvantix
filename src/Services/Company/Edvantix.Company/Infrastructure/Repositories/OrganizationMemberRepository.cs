using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

public sealed class OrganizationMemberRepository(IServiceProvider provider)
    : SoftDeleteRepository<OrganizationContext, OrganizationMember, Guid>(provider),
        IOrganizationMemberRepository;
