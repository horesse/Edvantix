using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.CleanUpSentEmailIntegrationEvent")]
public sealed record CleanUpSentEmailIntegrationEvent : IntegrationEvent;
