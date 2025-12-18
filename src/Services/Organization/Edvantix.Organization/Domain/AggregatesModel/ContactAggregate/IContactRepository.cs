using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Organization.Domain.AggregatesModel.ContactAggregate;

public interface IContactRepository : ICrudRepository<Contact, long>;
