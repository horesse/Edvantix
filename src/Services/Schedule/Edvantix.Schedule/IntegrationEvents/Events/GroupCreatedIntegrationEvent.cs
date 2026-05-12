using Wolverine.Attributes;

namespace Edvantix.Contracts;

/// <summary>
/// Получается от Organizational-сервиса при создании группы. Инициирует создание пустого <c>GroupSchedule</c>.
/// </summary>
[MessageIdentity("Edvantix.Contracts.GroupCreatedIntegrationEvent")]
public sealed record GroupCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid GroupId { get; init; }
    public required Guid OrganizationId { get; init; }
    public required DateOnly StartDate { get; init; }
}
