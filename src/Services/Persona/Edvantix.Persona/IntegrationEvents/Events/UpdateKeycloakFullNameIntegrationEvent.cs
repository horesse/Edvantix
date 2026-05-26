using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.UpdateKeycloakFullNameIntegrationEvent")]
public sealed record UpdateKeycloakFullNameIntegrationEvent(
    Guid AccountId,
    string FirstName,
    string LastName
) : IntegrationEvent;
