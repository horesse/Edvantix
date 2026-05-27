namespace Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

/// <summary>Активные группы, привязанные к одному из указанных уровней.</summary>
public sealed class GroupsByLevelIdsSpecification : Specification<Group>
{
    /// <param name="levelIds">Набор идентификаторов уровней для фильтрации.</param>
    public GroupsByLevelIdsSpecification(IReadOnlyCollection<Guid> levelIds)
    {
        Query.Where(g => !g.IsDeleted && levelIds.Contains(g.LevelId));
    }
}

/// <summary>Активные группы, привязанные к одному из указанных кабинетов.</summary>
public sealed class GroupsByRoomIdsSpecification : Specification<Group>
{
    /// <param name="roomIds">Набор идентификаторов кабинетов для фильтрации.</param>
    public GroupsByRoomIdsSpecification(IReadOnlyCollection<Guid> roomIds)
    {
        Query.Where(g => !g.IsDeleted && g.RoomId != null && roomIds.Contains(g.RoomId!.Value));
    }
}
