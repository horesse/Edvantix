using Wolverine.Attributes;

namespace Edvantix.Contracts;

/// <summary>
/// Контракт-двойник события создания организации из Organizational-сервиса.
/// Используется для сидирования дефолтных уровней в Groups-сервисе.
/// </summary>
[MessageIdentity("Edvantix.Contracts.OrganizationCreatedIntegrationEvent")]
public sealed record OrganizationCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid OrganizationId { get; init; }
    public required Guid OwnerProfileId { get; init; }
}
