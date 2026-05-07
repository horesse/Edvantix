using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.EnableKeycloakUserIntegrationEvent")]
public sealed record EnableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
