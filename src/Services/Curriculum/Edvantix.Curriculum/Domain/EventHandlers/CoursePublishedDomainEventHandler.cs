using Edvantix.Curriculum.Domain.Events;

namespace Edvantix.Curriculum.Domain.EventHandlers;

/// <summary>
/// Заглушка-хэндлер для события публикации курса.
/// Здесь можно добавить логику: инвалидация кэша, уведомление подписчиков.
/// </summary>
internal sealed class CoursePublishedDomainEventHandler
    : INotificationHandler<CoursePublishedDomainEvent>
{
    public ValueTask Handle(
        CoursePublishedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
