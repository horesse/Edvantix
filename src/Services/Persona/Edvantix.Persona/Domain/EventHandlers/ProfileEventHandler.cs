using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Persona.Domain.Events;
using Edvantix.Persona.Extensions;

namespace Edvantix.Persona.Domain.EventHandlers;

public sealed class ProfileEventHandlers(
    IEventDispatcher dispatcher,
    ILogger<ProfileEventHandlers> logger
) : INotificationHandler<ProfileRegisteredEvent>
{
    public async ValueTask Handle(
        ProfileRegisteredEvent notification,
        CancellationToken cancellationToken
    )
    {
        ProfileApiTrace.LogProfileRegistered(logger, notification.AccountId, notification.Login);
        await dispatcher.DispatchAsync(notification, cancellationToken);
    }
}
