using System.Security.Claims;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Constants.Permissions;
using Edvantix.Organizations.Grpc.Generated;
using Edvantix.Scheduling.Features.Schedule.GetSchedule;
using Edvantix.Scheduling.Grpc.Services;
using Edvantix.Scheduling.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Edvantix.Scheduling.UnitTests.Features;

/// <summary>
/// Unit tests for <see cref="GetScheduleQueryHandler"/> covering the three permission branches
/// (manager, teacher, student) and date range filtering.
/// Uses an in-memory EF Core database and mocked gRPC/group service dependencies.
/// </summary>
public sealed class GetScheduleQueryHandlerTests : IDisposable
{
    private static readonly Guid SchoolId = Guid.NewGuid();
    private static readonly Guid Teacher1Id = Guid.NewGuid();
    private static readonly Guid Teacher2Id = Guid.NewGuid();
    private static readonly Guid Group1Id = Guid.NewGuid();
    private static readonly Guid Group2Id = Guid.NewGuid();
    private static readonly DateTimeOffset BaseTime = new(2026, 9, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly SchedulingDbContext _dbContext;
    private readonly Mock<PermissionsGrpcService.PermissionsGrpcServiceClient> _grpcClientMock;
    private readonly Mock<IOrganizationsGroupService> _groupServiceMock;
    private readonly Mock<ITenantContext> _tenantContextMock;

    public GetScheduleQueryHandlerTests()
    {
        // Use a unique in-memory database per test instance to avoid cross-test contamination.
        var options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.Setup(t => t.SchoolId).Returns(SchoolId);
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);

        _dbContext = new SchedulingDbContext(options, _tenantContextMock.Object);

        _grpcClientMock = new Mock<PermissionsGrpcService.PermissionsGrpcServiceClient>();
        _groupServiceMock = new Mock<IOrganizationsGroupService>();

        // Default: group info returns placeholder values for any group.
        _groupServiceMock
            .Setup(s => s.GetGroupAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupInfoDto("Test Group", "#FF0000"));
    }

    public void Dispose() => _dbContext.Dispose();

    // -------------------------------------------------------------------------
    // Helper to seed LessonSlots directly into the in-memory context.
    // Bypasses tenant query filter by setting SchoolId explicitly on each slot.
    // -------------------------------------------------------------------------
    private async Task SeedSlots(params LessonSlot[] slots)
    {
        _dbContext.LessonSlots.AddRange(slots);
        await _dbContext.SaveChangesAsync();
    }

    private static LessonSlot MakeSlot(
        Guid teacherId,
        Guid groupId,
        DateTimeOffset start,
        DateTimeOffset end
    ) => new(SchoolId, groupId, teacherId, start, end);

    // -------------------------------------------------------------------------
    // Helper to build a ClaimsPrincipal with a profile_id claim.
    // -------------------------------------------------------------------------
    private static ClaimsPrincipal MakePrincipal(Guid profileId) =>
        new(
            new ClaimsIdentity(
                [new Claim(KeycloakClaimTypes.Profile, profileId.ToString())],
                "test"
            )
        );

    // -------------------------------------------------------------------------
    // Helper to set up the gRPC CheckPermission mock.
    // -------------------------------------------------------------------------
    private void SetupPermission(string permission, bool hasPermission)
    {
        var reply = new CheckPermissionReply { HasPermission = hasPermission };
        var call = GrpcTestHelpers.CreateUnaryCall(reply);

        _grpcClientMock
            .Setup(c =>
                c.CheckPermissionAsync(
                    It.Is<CheckPermissionRequest>(r => r.Permission == permission),
                    null,
                    null,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(call);
    }

    private GetScheduleQueryHandler CreateHandler(ClaimsPrincipal principal) =>
        new(
            _dbContext,
            _tenantContextMock.Object,
            principal,
            _grpcClientMock.Object,
            _groupServiceMock.Object
        );

    // -------------------------------------------------------------------------
    // Manager branch: returns all tenant slots with teacherId and studentCount.
    // -------------------------------------------------------------------------
    [Test]
    public async Task GivenManagerPermission_WhenGetSchedule_ThenAllTenantSlotsReturned()
    {
        var managerId = Guid.NewGuid();
        var start = BaseTime;

        await SeedSlots(
            MakeSlot(Teacher1Id, Group1Id, start, start.AddHours(1)),
            MakeSlot(Teacher1Id, Group2Id, start.AddHours(2), start.AddHours(3)),
            MakeSlot(Teacher2Id, Group1Id, start.AddHours(4), start.AddHours(5))
        );

        SetupPermission(SchedulingPermissions.CreateLessonSlot, hasPermission: true);

        var handler = CreateHandler(MakePrincipal(managerId));
        var query = new GetScheduleQuery(start.AddHours(-1), start.AddHours(6));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(3);
        result.ShouldAllBe(s => s.TeacherId.HasValue);
        result.ShouldAllBe(s => s.StudentCount.HasValue);
    }

    // -------------------------------------------------------------------------
    // Teacher branch: returns only own slots; TeacherId is null (hidden from teacher view).
    // -------------------------------------------------------------------------
    [Test]
    public async Task GivenTeacherPermission_WhenGetSchedule_ThenOnlyOwnSlotsReturned()
    {
        var start = BaseTime;

        await SeedSlots(
            MakeSlot(Teacher1Id, Group1Id, start, start.AddHours(1)),
            MakeSlot(Teacher1Id, Group2Id, start.AddHours(2), start.AddHours(3)),
            MakeSlot(Teacher2Id, Group1Id, start.AddHours(4), start.AddHours(5))
        );

        SetupPermission(SchedulingPermissions.CreateLessonSlot, hasPermission: false);
        SetupPermission(SchedulingPermissions.ViewOwnSchedule, hasPermission: true);

        var handler = CreateHandler(MakePrincipal(Teacher1Id));
        var query = new GetScheduleQuery(start.AddHours(-1), start.AddHours(6));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(2);
        result.ShouldAllBe(s => !s.TeacherId.HasValue);
        result.ShouldAllBe(s => s.StudentCount.HasValue);
    }

    // -------------------------------------------------------------------------
    // Student branch: returns only slots for groups the student belongs to.
    // -------------------------------------------------------------------------
    [Test]
    public async Task GivenStudentPermission_WhenGetSchedule_ThenOnlyGroupMembershipSlotsReturned()
    {
        var studentId = Guid.NewGuid();
        var start = BaseTime;

        await SeedSlots(
            MakeSlot(Teacher1Id, Group1Id, start, start.AddHours(1)),
            MakeSlot(Teacher1Id, Group2Id, start.AddHours(2), start.AddHours(3)),
            MakeSlot(Teacher2Id, Group2Id, start.AddHours(4), start.AddHours(5))
        );

        SetupPermission(SchedulingPermissions.CreateLessonSlot, hasPermission: false);
        SetupPermission(SchedulingPermissions.ViewOwnSchedule, hasPermission: false);

        // Student belongs to Group1 only.
        _groupServiceMock
            .Setup(s =>
                s.GetGroupsForStudentAsync(SchoolId, studentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([Group1Id]);

        var handler = CreateHandler(MakePrincipal(studentId));
        var query = new GetScheduleQuery(start.AddHours(-1), start.AddHours(6));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result[0].GroupId.ShouldBe(Group1Id);
        result[0].TeacherId.ShouldNotBeNull();
        result[0].StudentCount.ShouldBeNull();
    }

    // -------------------------------------------------------------------------
    // Date range filtering: only slots within the requested range are returned.
    // -------------------------------------------------------------------------
    [Test]
    public async Task GivenDateRange_WhenGetSchedule_ThenOnlySlotsInRangeReturned()
    {
        var managerId = Guid.NewGuid();
        var inRangeStart = BaseTime;
        var outOfRangeStart = BaseTime.AddDays(5);

        await SeedSlots(
            MakeSlot(Teacher1Id, Group1Id, inRangeStart, inRangeStart.AddHours(1)),
            MakeSlot(Teacher1Id, Group1Id, outOfRangeStart, outOfRangeStart.AddHours(1))
        );

        SetupPermission(SchedulingPermissions.CreateLessonSlot, hasPermission: true);

        var handler = CreateHandler(MakePrincipal(managerId));
        // Query covers only the first day.
        var query = new GetScheduleQuery(
            inRangeStart.AddHours(-1),
            inRangeStart.AddHours(2)
        );

        var result = await handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result[0].StartTime.ShouldBe(inRangeStart);
    }
}
