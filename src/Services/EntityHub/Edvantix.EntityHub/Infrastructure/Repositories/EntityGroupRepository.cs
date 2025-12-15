using Edvantix.Chassis.Repository.Crud;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityGroupAggregate;

namespace Edvantix.EntityHub.Infrastructure.Repositories;

public sealed class EntityGroupRepository(IServiceProvider provider)
    : CrudRepository<EntityHubContext, EntityGroup, long>(provider),
        IEntityGroupRepository;
