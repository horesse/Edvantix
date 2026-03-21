using Edvantix.Organizations.Grpc.Generated;

namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// gRPC-based implementation of <see cref="IOrganizationsGroupService"/>.
/// Calls the Organizations service via gRPC to validate groups and resolve student memberships.
///
/// Replaces the HTTP fallback from Plan 03-04 now that Organizations exposes the GetGroup
/// and GetGroupsForStudent RPCs (Plan 03-09).
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class OrganizationsGroupService(GroupsGrpcService.GroupsGrpcServiceClient grpcClient)
    : IOrganizationsGroupService
{
    /// <inheritdoc/>
    public async Task<bool> GroupExistsAsync(Guid groupId, CancellationToken ct)
    {
        // Use GetGroup RPC — returns Found=false when group is missing or soft-deleted.
        var reply = await grpcClient.GetGroupAsync(
            new GetGroupRequest { GroupId = groupId.ToString() },
            cancellationToken: ct
        );

        return reply.Found;
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetGroupsForStudentAsync(
        Guid schoolId,
        Guid profileId,
        CancellationToken ct
    )
    {
        var reply = await grpcClient.GetGroupsForStudentAsync(
            new GetGroupsForStudentRequest
            {
                SchoolId = schoolId.ToString(),
                ProfileId = profileId.ToString(),
            },
            cancellationToken: ct
        );

        return reply.GroupIds.Select(Guid.Parse).ToList();
    }

    /// <inheritdoc/>
    public async Task<GroupInfoDto?> GetGroupAsync(Guid groupId, CancellationToken ct)
    {
        var reply = await grpcClient.GetGroupAsync(
            new GetGroupRequest { GroupId = groupId.ToString() },
            cancellationToken: ct
        );

        if (!reply.Found)
        {
            return null;
        }

        return new GroupInfoDto(reply.Name, reply.Color);
    }
}
