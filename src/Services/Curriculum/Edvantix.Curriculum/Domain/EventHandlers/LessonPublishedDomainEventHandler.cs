using Edvantix.Curriculum.Domain.Events;

namespace Edvantix.Curriculum.Domain.EventHandlers;

/// <summary>
/// Заглушка-хэндлер для события публикации урока.
/// Здесь можно добавить логику: обновление TotalLessons-кэша, уведомление групп.
/// </summary>
internal sealed class LessonPublishedDomainEventHandler
    : INotificationHandler<LessonPublishedDomainEvent>
{
    public ValueTask Handle(
        LessonPublishedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
