using Edvantix.Audit.Domain.Events;

namespace Edvantix.Audit.Domain.EventHandlers;

/// <summary>
/// Обработчик события создания записи аудита.
/// Зарезервировано для будущих уведомлений или метрик.
/// </summary>
internal sealed class AuditEntryCreatedDomainEventHandler
    : INotificationHandler<AuditEntryCreatedDomainEvent>
{
    public ValueTask Handle(
        AuditEntryCreatedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
