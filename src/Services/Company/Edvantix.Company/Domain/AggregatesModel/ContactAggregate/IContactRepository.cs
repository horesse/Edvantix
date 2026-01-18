using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.ContactAggregate;

public interface IContactRepository : ICrudRepository<Contact, long>;
