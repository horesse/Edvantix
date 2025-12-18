using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;

public interface IOrganizationRepository : ICrudRepository<Organization, long>;

