using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.EventBus.Dispatcher;

public interface IEventMapper
{
    /// <summary>
    /// Преобразует доменное событие в соответствующее интеграционное событие.
    /// </summary>
    /// <param name="event">Доменное событие для преобразования.</param>
    /// <returns>Преобразованное интеграционное событие.</returns>
    IntegrationEvent MapToIntegrationEvent(DomainEvent @event);
}
