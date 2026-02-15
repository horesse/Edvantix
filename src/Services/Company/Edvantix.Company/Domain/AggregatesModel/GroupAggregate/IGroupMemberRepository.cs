using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate;

public interface IGroupMemberRepository : ICrudRepository<GroupMember, Guid>;
