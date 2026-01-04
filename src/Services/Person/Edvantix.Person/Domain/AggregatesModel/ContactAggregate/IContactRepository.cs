using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Person.Domain.AggregatesModel.ContactAggregate;

public interface IContactRepository : ICrudRepository<Contact, long>;
