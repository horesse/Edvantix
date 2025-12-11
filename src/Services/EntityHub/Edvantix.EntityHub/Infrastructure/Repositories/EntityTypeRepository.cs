using Edvantix.Chassis.Repository.Crud;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;

namespace Edvantix.EntityHub.Infrastructure.Repositories;

public class EntityTypeRepository(IServiceProvider provider)
    : CrudRepository<EntityHubContext, EntityType, long>(provider), IEntityTypeRepository;
