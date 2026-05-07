using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.LinkKeycloakProfileIntegrationEvent")]
public sealed record LinkKeycloakProfileIntegrationEvent(Guid AccountId, Guid ProfileId)
    : IntegrationEvent;
