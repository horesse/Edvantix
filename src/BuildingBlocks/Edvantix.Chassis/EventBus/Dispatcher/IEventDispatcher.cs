using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.EventBus.Dispatcher;

public interface IEventDispatcher
{
    /// <summary>
    /// Отправляет указанное доменное событие зарегистрированным обработчикам.
    /// </summary>
    /// <param name="event">Экземпляр доменного события для отправки.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции отправки.</param>
    /// <returns>Задача, представляющая асинхронную операцию отправки.</returns>
    Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет указанное доменное событие зарегистрированным обработчикам, передавая
    /// идентификатор инициировавшего пользователя через заголовки контекста публикации,
    /// чтобы он сохранялся через асинхронные границы (например, проекции Marten,
    /// фоновые демоны), где исходный <c>HttpContext</c> недоступен.
    /// </summary>
    /// <param name="event">Экземпляр доменного события для отправки.</param>
    /// <param name="userId">
    /// Идентификатор пользователя, добавляемый в заголовки результирующего интеграционного события.
    /// Если значение <see langword="null" /> или пустое, заголовок пользователя не устанавливается
    /// и вызов ведёт себя как <see cref="DispatchAsync(DomainEvent, CancellationToken)" />.
    /// </param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции отправки.</param>
    /// <returns>Задача, представляющая асинхронную операцию отправки.</returns>
    Task DispatchAsync(
        DomainEvent @event,
        string? userId,
        CancellationToken cancellationToken = default
    );
}
