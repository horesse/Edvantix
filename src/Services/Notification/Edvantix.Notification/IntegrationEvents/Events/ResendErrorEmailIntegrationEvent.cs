using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.ResendErrorEmailIntegrationEvent")]
public sealed record ResendErrorEmailIntegrationEvent : IntegrationEvent;
