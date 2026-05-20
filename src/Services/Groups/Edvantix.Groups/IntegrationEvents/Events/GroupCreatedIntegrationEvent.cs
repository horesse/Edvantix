using Wolverine.Attributes;

namespace Edvantix.Contracts;

/// <summary>
/// Публикуется при создании группы. Сигнализирует Schedule-сервису создать пустой <c>GroupSchedule</c>.
/// </summary>
[MessageIdentity("Edvantix.Contracts.GroupCreatedIntegrationEvent")]
public sealed record GroupCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid GroupId { get; init; }
    public required Guid OrganizationId { get; init; }
    public required DateOnly StartDate { get; init; }
}
