using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Constants.Permissions;
using Edvantix.Organizations.Grpc.Generated;
using Edvantix.Scheduling.Grpc.Services;
using Edvantix.Scheduling.Infrastructure;

namespace Edvantix.Scheduling.Features.Schedule.GetSchedule;

/// <summary>
/// Handles <see cref="GetScheduleQuery"/> by applying permission-based data filtering (D-07, D-08).
///
/// <para>Permission detection strategy:</para>
/// <list type="number">
///   <item><description>
///     Extract caller's <c>profile_id</c> from <see cref="ClaimsPrincipal"/>.
///   </description></item>
///   <item><description>
///     Make a gRPC call to check <c>scheduling.create-lesson-slot</c> (manager proxy).
///     Managers hold create permission; this is cheaper than a dedicated "is-manager" permission.
///   </description></item>
///   <item><description>
///     If not manager, make a second gRPC call to check <c>scheduling.view-own-schedule</c>
///     (teacher marker). Using a dedicated permission avoids the data-driven pitfall where
///     a teacher with no slots in the queried range would be misidentified as a student.
///   </description></item>
///   <item><description>
///     Student = neither manager nor teacher. Group IDs resolved via
///     <see cref="IOrganizationsGroupService.GetGroupsForStudentAsync"/>.
///   </description></item>
/// </list>
/// </summary>
public sealed class GetScheduleQueryHandler(
    SchedulingDbContext dbContext,
    ITenantContext tenantContext,
    ClaimsPrincipal user,
    PermissionsGrpcService.PermissionsGrpcServiceClient grpcClient,
    IOrganizationsGroupService groupService
) : IQueryHandler<GetScheduleQuery, List<ScheduleSlotDto>>
{
    /// <inheritdoc/>
    public async ValueTask<List<ScheduleSlotDto>> Handle(
        GetScheduleQuery query,
        CancellationToken cancellationToken
    )
    {
        // Step 1: Resolve caller identity.
        var profileIdClaim = user.FindFirst(KeycloakClaimTypes.Profile);
        if (profileIdClaim is null || !Guid.TryParse(profileIdClaim.Value, out var profileId))
        {
            // Baseline auth already gates the endpoint; if we reach here without a profileId
            // something is misconfigured — return empty rather than throwing.
            return [];
        }

        var schoolId = tenantContext.SchoolId;

        // Step 2: Determine role level via two gRPC permission checks.
        // Both calls are served from HybridCache (Phase 02) and are therefore cheap.
        var managerReply = await grpcClient.CheckPermissionAsync(
            new CheckPermissionRequest
            {
                UserId = profileId.ToString(),
                SchoolId = schoolId.ToString(),
                Permission = SchedulingPermissions.CreateLessonSlot,
            },
            cancellationToken: cancellationToken
        );

        var isManager = managerReply.HasPermission;

        var isTeacher = false;

        if (!isManager)
        {
            // Only check teacher permission when not already identified as manager.
            var teacherReply = await grpcClient.CheckPermissionAsync(
                new CheckPermissionRequest
                {
                    UserId = profileId.ToString(),
                    SchoolId = schoolId.ToString(),
                    Permission = SchedulingPermissions.ViewOwnSchedule,
                },
                cancellationToken: cancellationToken
            );

            isTeacher = teacherReply.HasPermission;
        }

        // Step 3: Query lesson slots with date range filter.
        // The tenant query filter is applied automatically via DbContext.OnModelCreating.
        var baseQuery = dbContext.LessonSlots.Where(s =>
            s.StartTime >= query.DateFrom && s.StartTime <= query.DateTo
        );

        List<LessonSlot> slots;

        if (isManager)
        {
            // Manager sees all tenant slots — no additional filter.
            slots = await baseQuery.ToListAsync(cancellationToken);
        }
        else if (isTeacher)
        {
            // Teacher sees only their own slots, identified by permission (not data-driven).
            slots = await baseQuery
                .Where(s => s.TeacherId == profileId)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // Student: resolve group memberships from Organizations service (D-15, SCH-05).
            var groupIds = await groupService.GetGroupsForStudentAsync(
                schoolId,
                profileId,
                cancellationToken
            );

            if (groupIds.Count == 0)
            {
                return [];
            }

            slots = await baseQuery
                .Where(s => groupIds.Contains(s.GroupId))
                .ToListAsync(cancellationToken);
        }

        if (slots.Count == 0)
        {
            return [];
        }

        // Step 4: Resolve group display info for each slot.
        // To avoid N+1, collect distinct group IDs and resolve in parallel.
        // Group info caching (Plan 03-09 gRPC swap) will make this efficient in production.
        var distinctGroupIds = slots.Select(s => s.GroupId).Distinct().ToList();
        var groupInfoTasks = distinctGroupIds.Select(gid =>
            groupService.GetGroupAsync(gid, cancellationToken)
        );
        var groupInfoArray = await Task.WhenAll(groupInfoTasks);

        // Build lookup: groupId → GroupInfoDto (or placeholder if Organizations returned null).
        var groupInfoLookup = distinctGroupIds
            .Zip(groupInfoArray)
            .ToDictionary(
                pair => pair.First,
                pair => pair.Second ?? new GroupInfoDto(pair.First.ToString(), "#808080")
            );

        // Step 5: Shape results based on permission level.
        return slots
            .Select(s =>
            {
                var groupInfo = groupInfoLookup[s.GroupId];

                return isManager
                        ? new ScheduleSlotDto(
                            Id: s.Id,
                            StartTime: s.StartTime,
                            EndTime: s.EndTime,
                            GroupId: s.GroupId,
                            GroupName: groupInfo.Name,
                            GroupColor: groupInfo.Color,
                            // Manager sees teacher identity and headcount.
                            TeacherId: s.TeacherId,
                            // v1 placeholder — full name resolution via Persona gRPC deferred to later plan.
                            TeacherName: s.TeacherId.ToString(),
                            // v1 placeholder — actual student count requires attendance data (Phase 4).
                            StudentCount: 0
                        )
                    : isTeacher
                        ? new ScheduleSlotDto(
                            Id: s.Id,
                            StartTime: s.StartTime,
                            EndTime: s.EndTime,
                            GroupId: s.GroupId,
                            GroupName: groupInfo.Name,
                            GroupColor: groupInfo.Color,
                            // Teacher does not need their own ID echoed back.
                            TeacherId: null,
                            TeacherName: null,
                            // v1 placeholder for student count.
                            StudentCount: 0
                        )
                    : new ScheduleSlotDto(
                        Id: s.Id,
                        StartTime: s.StartTime,
                        EndTime: s.EndTime,
                        GroupId: s.GroupId,
                        GroupName: groupInfo.Name,
                        GroupColor: groupInfo.Color,
                        // Student sees teacher identity (to know who is teaching).
                        TeacherId: s.TeacherId,
                        TeacherName: s.TeacherId.ToString(),
                        // Student does not need group headcount.
                        StudentCount: null
                    );
            })
            .ToList();
    }
}
