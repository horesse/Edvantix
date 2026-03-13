using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.EventBus.Kafka;

/// <summary>
///     A MassTransit publish filter that transparently routes outgoing
///     <see cref="ITopicProducer{TValue}" /> messages to Kafka via registered
///     <see cref="T" /> instances.
/// </summary>
/// <remarks>
///     When a Kafka producer is registered for the message type, the filter
///     produces the message to the corresponding Kafka topic and short-circuits
///     the InMemory bus pipeline. Messages without a registered producer (or
///     non-integration-event types) fall through to the InMemory transport.
/// </remarks>
/// <typeparam name="T">The message type being published.</typeparam>
internal sealed class KafkaPublishFilter<T>(IServiceProvider serviceProvider)
    : IFilter<PublishContext<T>>
    where T : class
{
    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        if (typeof(IntegrationEvent).IsAssignableFrom(typeof(T)))
        {
            var producer = serviceProvider.GetService<ITopicProducer<T>>();

            if (producer is not null)
            {
                await producer.Produce(context.Message, context.CancellationToken);
                return;
            }
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("kafkaPublish");
    }
}
