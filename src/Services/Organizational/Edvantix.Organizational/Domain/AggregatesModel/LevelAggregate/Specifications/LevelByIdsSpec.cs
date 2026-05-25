namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;

/// <summary>
/// Спецификация для пакетной загрузки уровней по набору идентификаторов.
/// </summary>
public sealed class LevelByIdsSpec : Specification<Level>
{
    /// <param name="ids">Идентификаторы уровней.</param>
    public LevelByIdsSpec(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        Query.Where(l => idList.Contains(l.Id) && !l.IsDeleted);
    }
}
