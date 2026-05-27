using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Grpc.Core;

namespace Edvantix.Groups.Grpc.Services;

/// <summary>gRPC-сервис Groups: утилитарные запросы по агрегату групп.</summary>
[ExcludeFromCodeCoverage]
internal sealed class GroupsService(IGroupRepository repository)
    : GroupsGrpcService.GroupsGrpcServiceBase
{
    /// <inheritdoc/>
    public override async Task<CountGroupsByDirectoryRefsResponse> CountGroupsByDirectoryRefs(
        CountGroupsByDirectoryRefsRequest request,
        ServerCallContext context
    )
    {
        var ids = request
            .Ids.Select(id => Guid.TryParse(id, out var g) ? g : (Guid?)null)
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToHashSet();

        var response = new CountGroupsByDirectoryRefsResponse();

        if (ids.Count == 0)
            return response;

        if (request.Kind == "Level")
        {
            var groups = await repository.ListAsync(
                new GroupsByLevelIdsSpecification(ids),
                context.CancellationToken
            );

            foreach (
                var (id, count) in groups.GroupBy(g => g.LevelId).Select(g => (g.Key, g.Count()))
            )
                response.Counts[id.ToString()] = count;
        }
        else if (request.Kind == "Room")
        {
            var groups = await repository.ListAsync(
                new GroupsByRoomIdsSpecification(ids),
                context.CancellationToken
            );

            foreach (
                var (id, count) in groups
                    .Where(g => g.RoomId.HasValue)
                    .GroupBy(g => g.RoomId!.Value)
                    .Select(g => (g.Key, g.Count()))
            )
                response.Counts[id.ToString()] = count;
        }
        else
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    $"Неизвестный тип справочника: {request.Kind}."
                )
            );
        }

        return response;
    }
}
