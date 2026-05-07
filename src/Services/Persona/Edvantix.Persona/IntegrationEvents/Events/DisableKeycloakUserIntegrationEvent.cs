using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.DisableKeycloakUserIntegrationEvent")]
public sealed record DisableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
