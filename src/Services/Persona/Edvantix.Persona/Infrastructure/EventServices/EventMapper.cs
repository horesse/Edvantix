using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Edvantix.Persona.Domain.Events;

namespace Edvantix.Persona.Infrastructure.EventServices;

internal sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
    {
        return @event switch
        {
            ProfileRegisteredEvent profileRegisteredEvent =>
                new SendInAppNotificationIntegrationEvent(
                    profileRegisteredEvent.AccountId,
                    NotificationType.Achievement,
                    "Добро пожаловать!",
                    "Вы успешно зарегистрировали свой профиль!"
                ),
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
    }
}
