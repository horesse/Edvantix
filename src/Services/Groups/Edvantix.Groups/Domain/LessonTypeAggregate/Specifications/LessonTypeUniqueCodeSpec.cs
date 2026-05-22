namespace Edvantix.Groups.Domain.LessonTypeAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности кода типа занятия в рамках организации
/// среди не архивных записей.
/// </summary>
public sealed class LessonTypeUniqueCodeSpec : Specification<LessonType>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="code">Код для проверки.</param>
    /// <param name="excludeId">Исключаемый идентификатор (для сценария update).</param>
    public LessonTypeUniqueCodeSpec(Guid organizationId, string code, Guid? excludeId = null)
    {
        Query.Where(lt =>
            lt.OrganizationId == organizationId && !lt.IsArchived && lt.Code == code
        );

        if (excludeId.HasValue)
            Query.Where(lt => lt.Id != excludeId.Value);
    }
}
