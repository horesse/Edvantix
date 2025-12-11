using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;

public interface IMicroserviceRepository : ICrudRepository<Microservice, long>;
