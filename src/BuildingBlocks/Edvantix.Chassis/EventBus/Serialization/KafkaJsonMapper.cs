using Confluent.Kafka;
using Wolverine;
using Wolverine.Kafka;
using Wolverine.Util;

namespace Edvantix.Chassis.EventBus.Serialization;

public class KafkaJsonMapper<TMessage> : IKafkaEnvelopeMapper
{
    private readonly string _messageTypeName = typeof(TMessage).ToMessageTypeName();

    public void MapEnvelopeToOutgoing(Envelope envelope, Message<string, byte[]> outgoing)
    {
        throw new NotSupportedException();
    }

    public void MapIncomingToEnvelope(Envelope envelope, Message<string, byte[]> incoming)
    {
        envelope.MessageType = _messageTypeName;

        envelope.ContentType = "application/json";

        envelope.Data = incoming.Value;
    }
}
