using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Groups.Grpc.Services;

/// <summary>gRPC-сервис статистики уровней для справочника Organizational.</summary>
[ExcludeFromCodeCoverage]
internal sealed class LevelStatsGrpcService(ILevelRepository repository)
    : GroupsGrpcService.GroupsGrpcServiceBase
{
    [AllowAnonymous]
    public override async Task<GetLevelStatsResponse> GetLevelStats(
        GetLevelStatsRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.OrgId, out var orgId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный идентификатор организации."));

        var (active, archived) = await repository.GetStatsAsync(orgId, context.CancellationToken);

        return new GetLevelStatsResponse
        {
            ActiveCount = active,
            ArchivedCount = archived,
        };
    }
}
