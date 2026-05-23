namespace Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности названия предмета среди активных записей организации.
/// </summary>
public sealed class SubjectByNameSpec : Specification<Subject>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="name">Название для поиска (будет обрезано через Trim).</param>
    /// <param name="excludeId">Идентификатор предмета, исключаемого из проверки (для update-сценария).</param>
    public SubjectByNameSpec(Guid organizationId, string name, Guid? excludeId = null)
    {
        var trimmed = name.Trim();

        Query.Where(s => s.OrganizationId == organizationId && !s.IsArchived && s.Name == trimmed);

        if (excludeId.HasValue)
            Query.Where(s => s.Id != excludeId.Value);
    }
}
