using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;

public interface IContactRepository : ICrudRepository<Contact, long>;

