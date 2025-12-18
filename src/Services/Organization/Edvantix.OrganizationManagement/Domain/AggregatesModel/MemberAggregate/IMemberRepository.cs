using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;

public interface IMemberRepository : ICrudRepository<Member, Guid>;
