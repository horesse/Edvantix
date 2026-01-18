using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.MemberAggregate;

public interface IMemberRepository : ICrudRepository<Member, Guid>;
