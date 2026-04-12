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
}
