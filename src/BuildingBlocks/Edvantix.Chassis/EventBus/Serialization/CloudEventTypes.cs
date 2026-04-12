namespace Edvantix.Chassis.EventBus.Serialization;

internal static class CloudEventTypes
{
    private const string Prefix = "urn:message:";

    internal static string FromMessageTypes(string[] messageTypes)
    {
        if (messageTypes.Length == 0)
        {
            return "com.masstransit.message";
        }

        // URN MassTransit имеют вид "urn:message:Namespace:TypeName"
        // Преобразует в тип CloudEvent через точку, например "Namespace.TypeName"
        var primary = messageTypes[0];
        return primary.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)
            ? primary[Prefix.Length..].Replace(':', '.')
            : primary;
    }
}
