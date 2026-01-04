using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;

public interface IPersonInfoRepository : ICrudRepository<PersonInfo, long>;
