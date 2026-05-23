namespace Edvantix.Groups.Domain.LessonTypeAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности имени типа занятия в рамках организации
/// среди не архивных записей.
/// </summary>
public sealed class LessonTypeUniqueNameSpec : Specification<LessonType>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="name">Имя для проверки.</param>
    /// <param name="excludeId">Исключаемый идентификатор (для сценария update).</param>
    public LessonTypeUniqueNameSpec(Guid organizationId, string name, Guid? excludeId = null)
    {
        Query.Where(lt => lt.OrganizationId == organizationId && !lt.IsArchived && lt.Name == name);

        if (excludeId.HasValue)
            Query.Where(lt => lt.Id != excludeId.Value);
    }
}
