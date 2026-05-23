namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

/// <summary>
/// Фабрика дефолтных (системных) статусов студентов, создаваемых при регистрации организации.
/// <para>Создаёт 4 системных статуса: Активный, В академе, Выпускник, Отчислен.</para>
/// </summary>
public static class DefaultStudentStatusesFactory
{
    /// <summary>Создаёт набор системных статусов для указанной организации.</summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    public static IReadOnlyList<StudentStatus> CreateFor(Guid organizationId) =>
        [
            new StudentStatus(
                organizationId,
                "Активный",
                "ACTIVE",
                StudentStatusTone.Active,
                isSystem: true,
                order: 0
            ),
            new StudentStatus(
                organizationId,
                "В академе",
                "ON_LEAVE",
                StudentStatusTone.Warning,
                isSystem: true,
                order: 1
            ),
            new StudentStatus(
                organizationId,
                "Выпускник",
                "GRADUATE",
                StudentStatusTone.Neutral,
                isSystem: true,
                order: 2
            ),
            new StudentStatus(
                organizationId,
                "Отчислен",
                "EXPELLED",
                StudentStatusTone.Inactive,
                isSystem: true,
                order: 3
            ),
        ];
}
