namespace Edvantix.Organizational.Features.Directories.LessonTypes;

/// <summary>
/// Проверяет уникальность имени и кода типа занятия в рамках организации.
/// </summary>
public interface ILessonTypeUniqueChecker
{
    /// <summary>
    /// Проверяет, существует ли не архивная запись с данным именем в организации.
    /// </summary>
    Task<bool> NameExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    );

    /// <summary>
    /// Проверяет, существует ли не архивная запись с данным кодом в организации.
    /// </summary>
    Task<bool> CodeExistsAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken ct
    );
}
