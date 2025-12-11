using Edvantix.Chassis.Repository.Crud;
using Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;

namespace Edvantix.EntityHub.Infrastructure.Repositories;

public class MicroserviceRepository(IServiceProvider provider)
    : CrudRepository<EntityHubContext, Microservice, long>(provider), IMicroserviceRepository;
