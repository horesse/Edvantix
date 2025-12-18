using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Organization.Domain.AggregatesModel.MemberAggregate;

public interface IMemberRepository : ICrudRepository<Member, Guid>;
