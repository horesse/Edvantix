using Edvantix.Groups.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Groups;

/// <summary>Обёртка gRPC-клиента Groups-сервиса.</summary>
[ExcludeFromCodeCoverage]
internal sealed class GroupsService(GroupsGrpcService.GroupsGrpcServiceClient client)
    : IGroupsService
{
    public async Task<(int Active, int Archived)> GetLevelStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.GetLevelStatsAsync(
            new GetLevelStatsRequest { OrgId = organizationId.ToString("D") },
            cancellationToken: cancellationToken
        );

        return (response.ActiveCount, response.ArchivedCount);
    }
}
