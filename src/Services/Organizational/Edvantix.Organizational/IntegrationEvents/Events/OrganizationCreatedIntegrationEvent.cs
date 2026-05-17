using Wolverine.Attributes;

namespace Edvantix.Contracts;

/// <summary>
/// Публикуется при создании организации. Сигнализирует Groups-сервису сидировать дефолтные уровни.
/// </summary>
[MessageIdentity("Edvantix.Contracts.OrganizationCreatedIntegrationEvent")]
public sealed record OrganizationCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid OrganizationId { get; init; }
    public required Guid OwnerProfileId { get; init; }
}
