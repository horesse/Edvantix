using Edvantix.Organizations.Grpc.Generated;
using Edvantix.Organizations.Infrastructure;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Organizations.Grpc.Services;

/// <summary>
/// gRPC server implementation of the <c>GroupsGrpcService</c> contract.
/// Provides group membership resolution and group lookup for downstream services (primarily Scheduling).
///
/// NOTE: Both RPCs use <c>IgnoreQueryFilters</c> on cross-tenant lookups because gRPC calls
/// lack an HTTP tenant context header. <c>GetGroupsForStudent</c> applies explicit schoolId filtering;
/// <c>GetGroup</c> looks up by ID only and is safe since GroupId is not guessable.
/// </summary>
public sealed class GroupsGrpcService(OrganizationsDbContext dbContext)
    : Generated.GroupsGrpcService.GroupsGrpcServiceBase
{
    /// <summary>
    /// Returns the IDs of all groups a student belongs to in a given school.
    /// </summary>
    /// <param name="request">Contains <c>SchoolId</c> and <c>ProfileId</c>.</param>
    /// <param name="context">The server call context.</param>
    public override async Task<GetGroupsForStudentReply> GetGroupsForStudent(
        GetGroupsForStudentRequest request,
        ServerCallContext context
    )
    {
        var schoolId = Guid.Parse(request.SchoolId);
        var profileId = Guid.Parse(request.ProfileId);

        // IgnoreQueryFilters because gRPC context has no X-School-Id header (no ITenantContext).
        // Explicit schoolId filter provides the tenant scoping instead.
        var groupIds = await dbContext
            .GroupMemberships.IgnoreQueryFilters()
            .Where(m => m.SchoolId == schoolId && m.ProfileId == profileId)
            .Select(m => m.GroupId.ToString())
            .ToListAsync(context.CancellationToken);

        var reply = new GetGroupsForStudentReply();
        reply.GroupIds.AddRange(groupIds);

        return reply;
    }

    /// <summary>
    /// Returns group details by ID. Returns <c>Found = false</c> when the group does not exist or is deleted.
    /// </summary>
    /// <param name="request">Contains <c>GroupId</c>.</param>
    /// <param name="context">The server call context.</param>
    public override async Task<GetGroupReply> GetGroup(
        GetGroupRequest request,
        ServerCallContext context
    )
    {
        var groupId = Guid.Parse(request.GroupId);

        // IgnoreQueryFilters because gRPC context has no ITenantContext.
        // Scheduling knows the GroupId and only needs to verify it exists and is not deleted.
        var group = await dbContext
            .Groups.IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted, context.CancellationToken);

        if (group is null)
        {
            return new GetGroupReply { Found = false };
        }

        return new GetGroupReply
        {
            Id = group.Id.ToString(),
            Name = group.Name,
            Color = group.Color,
            MaxCapacity = group.MaxCapacity,
            Found = true
        };
    }
}
