using Edvantix.SharedKernel.SeedWork;
using Wolverine;

namespace Edvantix.Chassis.EventBus.Dispatcher;

internal sealed class EventDispatcher(IMessageBus bus, IEventMapper eventMapper) : IEventDispatcher
{
    public Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default)
    {
        return DispatchAsync(@event, null, cancellationToken);
    }

    public async Task DispatchAsync(
        DomainEvent @event,
        string? userId,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(@event);

        var integrationEvent =
            eventMapper.MapToIntegrationEvent(@event)
            ?? throw new InvalidOperationException(
                $"No integration event mapping found for '{@event.GetType().Name}'."
            );

        if (string.IsNullOrEmpty(userId))
        {
            await bus.PublishAsync((object)integrationEvent);
            return;
        }

        // Передаём идентификатор пользователя через DeliveryOptions, чтобы правило конверта
        // (UserIdEnvelopeMiddleware) и политика заголовков CloudEvent подхватили его
        // при сериализации исходящего сообщения Kafka (FR-009).
        var deliveryOptions = new DeliveryOptions();
        deliveryOptions.Headers.Add(EventBusHeaders.UserId, userId);
        deliveryOptions.Headers.Add("userid", userId);

        await bus.PublishAsync((object)integrationEvent, deliveryOptions);
    }
}
