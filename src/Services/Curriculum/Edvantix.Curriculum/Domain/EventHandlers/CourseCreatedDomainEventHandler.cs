using Edvantix.Curriculum.Domain.Events;

namespace Edvantix.Curriculum.Domain.EventHandlers;

/// <summary>
/// Заглушка-хэндлер для события создания курса.
/// Здесь можно добавить логику: аналитика, уведомления, первичная инициализация.
/// </summary>
internal sealed class CourseCreatedDomainEventHandler
    : INotificationHandler<CourseCreatedDomainEvent>
{
    public ValueTask Handle(
        CourseCreatedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
