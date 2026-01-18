using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;

public interface IProfileRepository : ICrudRepository<Profile, long>;
