using Edvantix.Chassis.Specification;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.UnitTests.OrganizationPermissionService;

/// <summary>
/// Unit-тесты для <see cref="Edvantix.Organizational.Infrastructure.Services.OrganizationPermissionService"/>.
/// Проверяются граничные случаи иерархии ролей и матрицы доступа.
/// </summary>
public sealed class OrganizationPermissionServiceTests
{
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock;
    private readonly IOrganizationPermissionMatrix _matrix;
    private readonly Edvantix.Organizational.Infrastructure.Services.OrganizationPermissionService _sut;

    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid AdminId = Guid.NewGuid();
    private static readonly Guid ManagerId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();
    private static readonly Guid StudentId = Guid.NewGuid();
    private static readonly Guid NonMemberId = Guid.NewGuid();

    public OrganizationPermissionServiceTests()
    {
        _memberRepoMock = new Mock<IOrganizationMemberRepository>();
        _matrix = new OrganizationPermissionMatrix();
        _sut = new Edvantix.Organizational.Infrastructure.Services.OrganizationPermissionService(
            _memberRepoMock.Object,
            _matrix
        );

        // Owner в данном тесте использует OrganizationRole.Owner,
        // маппится на OrganizationBaseRole.Owner.
        SetupMember(OwnerId, OrganizationRole.Owner);
        SetupMember(ManagerId, OrganizationRole.Manager);
        SetupMember(TeacherId, OrganizationRole.Teacher);
        SetupMember(StudentId, OrganizationRole.Student);

        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.Is<ISpecification<OrganizationMember>>(s => true),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<ISpecification<OrganizationMember>, CancellationToken>(
                (spec, _) =>
                {
                    // Simulate member lookup based on the test setup
                    return Task.FromResult<OrganizationMember?>(null);
                }
            );
    }

    private void SetupMember(Guid profileId, OrganizationRole role)
    {
        var member = new OrganizationMember(OrgId, profileId, role);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.Is<OrganizationMemberSpecification>(s => true),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);
    }

    // ─── GetEffectiveRoleAsync ────────────────────────────────────────────────────

    [Test]
    public async Task GivenOwnerMember_WhenGetEffectiveRole_ThenReturnsOwnerBaseRole()
    {
        var member = new OrganizationMember(OrgId, OwnerId, OrganizationRole.Owner);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.GetEffectiveRoleAsync(OwnerId, OrgId);

        result.ShouldBe(OrganizationBaseRole.Owner);
    }

    [Test]
    public async Task GivenManagerMember_WhenGetEffectiveRole_ThenReturnsManagerBaseRole()
    {
        var member = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.GetEffectiveRoleAsync(ManagerId, OrgId);

        result.ShouldBe(OrganizationBaseRole.Manager);
    }

    [Test]
    public async Task GivenNonMember_WhenGetEffectiveRole_ThenReturnsNull()
    {
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((OrganizationMember?)null);

        var result = await _sut.GetEffectiveRoleAsync(NonMemberId, OrgId);

        result.ShouldBeNull();
    }

    // ─── HasPermissionAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenOwner_WhenCheckOrganizationManageCustomRoles_ThenHasPermission()
    {
        var member = new OrganizationMember(OrgId, OwnerId, OrganizationRole.Owner);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.HasPermissionAsync(
            OwnerId,
            OrgId,
            Permission.OrganizationManageCustomRoles
        );

        result.ShouldBeTrue();
    }

    [Test]
    public async Task GivenManager_WhenCheckOrganizationManageCustomRoles_ThenHasNoPermission()
    {
        var member = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.HasPermissionAsync(
            ManagerId,
            OrgId,
            Permission.OrganizationManageCustomRoles
        );

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenStudent_WhenCheckGroupView_ThenHasPermission()
    {
        var member = new OrganizationMember(OrgId, StudentId, OrganizationRole.Student);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.HasPermissionAsync(StudentId, OrgId, Permission.GroupView);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task GivenStudent_WhenCheckMemberManage_ThenHasNoPermission()
    {
        var member = new OrganizationMember(OrgId, StudentId, OrganizationRole.Student);
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(member);

        var result = await _sut.HasPermissionAsync(StudentId, OrgId, Permission.MemberManage);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenNonMember_WhenCheckAnyPermission_ThenHasNoPermission()
    {
        _memberRepoMock
            .Setup(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((OrganizationMember?)null);

        var result = await _sut.HasPermissionAsync(NonMemberId, OrgId, Permission.GroupView);

        result.ShouldBeFalse();
    }

    // ─── CanManageUserAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenOwnerVsManager_WhenCanManageUser_ThenOwnerCanManageManager()
    {
        var ownerMember = new OrganizationMember(OrgId, OwnerId, OrganizationRole.Owner);
        var managerMember = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ownerMember)
            .ReturnsAsync(managerMember);

        var result = await _sut.CanManageUserAsync(OwnerId, ManagerId, OrgId);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task GivenManagerVsOwner_WhenCanManageUser_ThenManagerCannotManageOwner()
    {
        var managerMember = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);
        var ownerMember = new OrganizationMember(OrgId, OwnerId, OrganizationRole.Owner);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(managerMember)
            .ReturnsAsync(ownerMember);

        var result = await _sut.CanManageUserAsync(ManagerId, OwnerId, OrgId);

        // Manager не имеет RoleAssign → не может управлять никем.
        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenManagerVsTeacher_WhenCanManageUser_ThenManagerCannotManageTeacher()
    {
        var managerMember = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);
        var teacherMember = new OrganizationMember(OrgId, TeacherId, OrganizationRole.Teacher);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(managerMember)
            .ReturnsAsync(teacherMember);

        // Manager не имеет Permission.RoleAssign — не может управлять ролями.
        var result = await _sut.CanManageUserAsync(ManagerId, TeacherId, OrgId);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenTeacherVsStudent_WhenCanManageUser_ThenTeacherCannotManageStudent()
    {
        var teacherMember = new OrganizationMember(OrgId, TeacherId, OrganizationRole.Teacher);
        var studentMember = new OrganizationMember(OrgId, StudentId, OrganizationRole.Student);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(teacherMember)
            .ReturnsAsync(studentMember);

        var result = await _sut.CanManageUserAsync(TeacherId, StudentId, OrgId);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenOwnerVsOwner_WhenCanManageUser_ThenOwnerCannotManagePeerOwner()
    {
        var ownerMember1 = new OrganizationMember(OrgId, OwnerId, OrganizationRole.Owner);
        var ownerMember2 = new OrganizationMember(OrgId, Guid.NewGuid(), OrganizationRole.Owner);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ownerMember1)
            .ReturnsAsync(ownerMember2);

        // Owner(0) vs Owner(0): CanManageRole требует строго меньше.
        var result = await _sut.CanManageUserAsync(OwnerId, ownerMember2.ProfileId, OrgId);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenActorIsNonMember_WhenCanManageUser_ThenReturnsFalse()
    {
        var targetMember = new OrganizationMember(OrgId, ManagerId, OrganizationRole.Manager);

        _memberRepoMock
            .SetupSequence(r =>
                r.FindAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((OrganizationMember?)null)
            .ReturnsAsync(targetMember);

        var result = await _sut.CanManageUserAsync(NonMemberId, ManagerId, OrgId);

        result.ShouldBeFalse();
    }
}
