using Wolverine.Attributes;

namespace Edvantix.Contracts;

/// <summary>
/// Курс переведён в архив.
/// Подписчики (Organizational) должны отметить привязанные группы
/// как использующие архивный курс.
/// </summary>
[MessageIdentity("Edvantix.Contracts.CourseArchivedIntegrationEvent")]
public sealed record CourseArchivedIntegrationEvent : IntegrationEvent
{
    /// <summary>Идентификатор архивированного курса.</summary>
    public required Guid CourseId { get; init; }

    /// <summary>Идентификатор организации-владельца.</summary>
    public required Guid OrganizationId { get; init; }
}
