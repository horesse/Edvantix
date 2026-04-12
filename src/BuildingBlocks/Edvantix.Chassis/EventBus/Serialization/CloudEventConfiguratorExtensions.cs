using MassTransit;

namespace Edvantix.Chassis.EventBus.Serialization;

public static class CloudEventConfiguratorExtensions
{
    extension(IBusFactoryConfigurator configurator)
    {
        /// <summary>
        /// Настраивает конфигуратор MassTransit для использования CloudEvents при сериализации и десериализации сообщений.
        /// </summary>
        /// <remarks>
        /// Единственный экземпляр <see cref="CloudEventSerializerFactory" /> регистрируется и для сериализатора, и для десериализатора
        /// для обеспечения согласованной обработки полезной нагрузки.
        /// </remarks>
        public void UseCloudEvents()
        {
            var factory = new CloudEventSerializerFactory();
            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }
    }

    extension(IReceiveEndpointConfigurator configurator)
    {
        /// <summary>
        /// Настраивает эндпоинт получения для использования CloudEvents при сериализации и десериализации сообщений.
        /// </summary>
        /// <remarks>
        /// Единственный экземпляр <see cref="CloudEventSerializerFactory" /> используется и для сериализатора, и для десериализатора
        /// для согласованной обработки полезной нагрузки на этом эндпоинте.
        /// </remarks>
        public void UseCloudEvents()
        {
            var factory = new CloudEventSerializerFactory();
            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }
    }
}
