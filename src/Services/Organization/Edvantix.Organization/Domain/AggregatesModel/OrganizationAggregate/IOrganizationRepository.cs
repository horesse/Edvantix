using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Organization.Domain.AggregatesModel.OrganizationAggregate;

public interface IOrganizationRepository : ICrudRepository<Organization, long>;
