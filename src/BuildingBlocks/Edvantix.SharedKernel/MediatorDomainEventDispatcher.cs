using System.Collections.Immutable;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.SharedKernel;

public sealed class MediatorDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(ImmutableList<IHasDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            if (entity is not HasDomainEvents hasDomainEvents)
            {
                continue;
            }

            DomainEvent[] events = [.. hasDomainEvents.DomainEvents];
            hasDomainEvents.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await publisher.Publish(domainEvent);
            }
        }
    }
}

public static class MediatorDomainEventDispatcherExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует <see cref="MediatorDomainEventDispatcher" /> как scoped-реализацию
        /// <see cref="IDomainEventDispatcher" /> в контейнере зависимостей.
        /// </summary>
        public void AddMediatorDomainEventDispatcher()
        {
            services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();
        }
    }
}
