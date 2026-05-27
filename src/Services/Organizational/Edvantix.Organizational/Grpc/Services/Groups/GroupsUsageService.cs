using Edvantix.Groups.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Groups;

/// <summary>gRPC-клиент для получения статистики использования справочников из сервиса Groups.</summary>
[ExcludeFromCodeCoverage]
internal sealed class GroupsUsageService(GroupsGrpcService.GroupsGrpcServiceClient client)
    : IGroupsUsageService
{
    private const string LevelKind = "Level";
    private const string RoomKind = "Room";

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<Guid, int>> CountByLevelIdsAsync(
        IEnumerable<Guid> levelIds,
        CancellationToken cancellationToken = default
    ) => CountByKindAsync(LevelKind, levelIds, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<Guid, int>> CountByRoomIdsAsync(
        IEnumerable<Guid> roomIds,
        CancellationToken cancellationToken = default
    ) => CountByKindAsync(RoomKind, roomIds, cancellationToken);

    private async Task<IReadOnlyDictionary<Guid, int>> CountByKindAsync(
        string kind,
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        var request = new CountGroupsByDirectoryRefsRequest { Kind = kind };
        request.Ids.AddRange(ids.Select(id => id.ToString()));

        var response = await client.CountGroupsByDirectoryRefsAsync(
            request,
            cancellationToken: cancellationToken
        );

        return response.Counts.ToDictionary(kvp => Guid.Parse(kvp.Key), kvp => kvp.Value);
    }
}
