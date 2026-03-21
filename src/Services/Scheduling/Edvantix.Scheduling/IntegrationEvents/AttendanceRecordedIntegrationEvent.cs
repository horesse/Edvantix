// This file lives in the Scheduling assembly but uses namespace Edvantix.Contracts — the established pattern
// (same as UserPermissionsInvalidatedIntegrationEvent in Organizations). The namespace ensures
// DiscoverMessageTypes finds it during RegisterKafkaProducers assembly scan.
// Kafka topic: attendance-recorded-integration-event (auto-derived per D-11 by KebabCaseEndpointNameFormatter).
namespace Edvantix.Contracts;

/// <summary>
/// Integration event published to the <c>attendance-recorded-integration-event</c> Kafka topic
/// when a student's attendance is marked or updated.
/// <para>
/// Phase 5 (Payments) MUST subscribe to the <c>attendance-recorded-integration-event</c> topic.
/// Topic name is auto-derived by <c>KebabCaseEndpointNameFormatter.SanitizeName</c> per D-11.
/// </para>
/// </summary>
public sealed record AttendanceRecordedIntegrationEvent(
    Guid StudentId,
    Guid LessonSlotId,
    Guid SchoolId,
    string Status,
    DateTimeOffset Timestamp,
    Guid CorrelationId
) : IntegrationEvent;
