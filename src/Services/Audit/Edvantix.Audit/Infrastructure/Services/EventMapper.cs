using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Audit.Infrastructure.Services;

/// <summary>
/// Маппер доменных событий в интеграционные события шины сообщений.
/// Записи аудита не публикуют интеграционные события — они сами являются
/// результатом обработки событий из других сервисов.
/// </summary>
public sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event) =>
        throw new ArgumentOutOfRangeException(nameof(@event), @event, null);
}
