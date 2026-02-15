using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;

public interface IOrganizationMemberRepository : ICrudRepository<OrganizationMember, Guid>;
