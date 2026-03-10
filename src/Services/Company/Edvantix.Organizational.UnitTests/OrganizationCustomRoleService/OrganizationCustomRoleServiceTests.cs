using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Repository;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Infrastructure.Services;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.UnitTests.OrganizationCustomRoleService;

public sealed class OrganizationCustomRoleServiceTests
{
    private readonly Mock<IOrganizationCustomRoleRepository> _roleRepoMock;
    private readonly Mock<IOrganizationAuthorizationService> _authServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Edvantix.Organizational.Features.OrganizationCustomRoleFeature.OrganizationCustomRoleService _sut;

    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid RoleId = Guid.NewGuid();
    private static readonly Guid ProfileId = Guid.NewGuid();

    public OrganizationCustomRoleServiceTests()
    {
        _roleRepoMock = new Mock<IOrganizationCustomRoleRepository>();
        _authServiceMock = new Mock<IOrganizationAuthorizationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _roleRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var ownerMember = new OrganizationMember(OrgId, ProfileId, OrganizationRole.Owner);
        _authServiceMock
            .Setup(a =>
                a.RequireOrgRoleAsync(
                    OrgId,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<OrganizationRole[]>()
                )
            )
            .ReturnsAsync(ownerMember);
        _authServiceMock
            .Setup(a => a.GetCurrentMemberAsync(OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerMember);

        _sut =
            new Edvantix.Organizational.Features.OrganizationCustomRoleFeature.OrganizationCustomRoleService(
                _roleRepoMock.Object,
                _authServiceMock.Object
            );
    }

    // ─── CreateAsync ─────────────────────────────────────────────────────────────

    [Test]
    public async Task GivenUniqueCode_WhenCreateAsync_ThenAddAsyncAndSaveAreCalled()
    {
        _roleRepoMock
            .Setup(r => r.FindByCodeAsync(OrgId, "teacher-advanced", It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationCustomRole?)null);

        _roleRepoMock
            .Setup(r =>
                r.AddAsync(It.IsAny<OrganizationCustomRole>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((OrganizationCustomRole role, CancellationToken _) => role);

        // ID генерируется БД (uuidv7) — в юнит-тестах без EF будет Guid.Empty.
        // Проверяем, что репозиторий и UoW были вызваны корректно.
        await _sut.CreateAsync(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            "Advanced teacher",
            CancellationToken.None
        );

        _roleRepoMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<OrganizationCustomRole>(role =>
                        role.OrganizationId == OrgId
                        && role.Code == "teacher-advanced"
                        && role.BaseRole == OrganizationBaseRole.Teacher
                        && role.Description == "Advanced teacher"
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenDuplicateCode_WhenCreateAsync_ThenThrowsInvalidOperationException()
    {
        var existing = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher
        );
        _roleRepoMock
            .Setup(r => r.FindByCodeAsync(OrgId, "teacher-advanced", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var act = () =>
            _sut.CreateAsync(
                OrgId,
                "teacher-advanced",
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<InvalidOperationException>();
        _roleRepoMock.Verify(
            r => r.AddAsync(It.IsAny<OrganizationCustomRole>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenNonOwnerUser_WhenCreateAsync_ThenThrowsForbiddenException()
    {
        _authServiceMock
            .Setup(a =>
                a.RequireOrgRoleAsync(
                    OrgId,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<OrganizationRole[]>()
                )
            )
            .ThrowsAsync(new ForbiddenException("Доступ запрещён."));

        var act = () =>
            _sut.CreateAsync(
                OrgId,
                "teacher-advanced",
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    // ─── UpdateAsync ─────────────────────────────────────────────────────────────

    [Test]
    public async Task GivenSameCode_WhenUpdateAsync_ThenRoleIsUpdatedWithoutAssignmentCheck()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            "Old"
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await _sut.UpdateAsync(
            RoleId,
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Admin,
            "Updated",
            CancellationToken.None
        );

        // Код не менялся — проверка назначений не должна вызываться.
        _roleRepoMock.Verify(
            r => r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNewCodeAndNoAssignedUsers_WhenUpdateAsync_ThenCodeIsUpdated()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "old-code",
            OrganizationBaseRole.Teacher,
            null
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock
            .Setup(r =>
                r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);
        _roleRepoMock
            .Setup(r => r.FindByCodeAsync(OrgId, "new-code", It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationCustomRole?)null);

        await _sut.UpdateAsync(
            RoleId,
            OrgId,
            "new-code",
            OrganizationBaseRole.Teacher,
            null,
            CancellationToken.None
        );

        role.Code.ShouldBe("new-code");
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNewCodeAndAssignedUsers_WhenUpdateAsync_ThenThrowsInvalidOperationException()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "old-code",
            OrganizationBaseRole.Teacher,
            null
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock
            .Setup(r =>
                r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(5);

        var act = () =>
            _sut.UpdateAsync(
                RoleId,
                OrgId,
                "new-code",
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenNewCodeConflict_WhenUpdateAsync_ThenThrowsInvalidOperationException()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "old-code",
            OrganizationBaseRole.Teacher,
            null
        );
        var conflictRole = new OrganizationCustomRole(
            OrgId,
            "new-code",
            OrganizationBaseRole.Admin,
            null
        );

        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock
            .Setup(r =>
                r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);
        _roleRepoMock
            .Setup(r => r.FindByCodeAsync(OrgId, "new-code", It.IsAny<CancellationToken>()))
            .ReturnsAsync(conflictRole);

        var act = () =>
            _sut.UpdateAsync(
                RoleId,
                OrgId,
                "new-code",
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GivenNonExistentRole_WhenUpdateAsync_ThenThrowsNotFoundException()
    {
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationCustomRole?)null);

        var act = () =>
            _sut.UpdateAsync(
                RoleId,
                OrgId,
                "any-code",
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    // ─── DeleteAsync ─────────────────────────────────────────────────────────────

    [Test]
    public async Task GivenRoleWithNoAssignedUsers_WhenDeleteAsync_ThenRoleIsSoftDeleted()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            null
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock
            .Setup(r =>
                r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        await _sut.DeleteAsync(RoleId, OrgId, CancellationToken.None);

        role.IsDeleted.ShouldBeTrue();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenRoleWithAssignedUsers_WhenDeleteAsync_ThenThrowsInvalidOperationException()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            null
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock
            .Setup(r =>
                r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(3);

        var act = () => _sut.DeleteAsync(RoleId, OrgId, CancellationToken.None);

        await act.ShouldThrowAsync<InvalidOperationException>();
        role.IsDeleted.ShouldBeFalse();
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenNonExistentRole_WhenDeleteAsync_ThenThrowsNotFoundException()
    {
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationCustomRole?)null);

        var act = () => _sut.DeleteAsync(RoleId, OrgId, CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    // ─── ListAsync ───────────────────────────────────────────────────────────────

    [Test]
    public async Task GivenOrganizationMember_WhenListAsync_ThenReturnsActiveRoles()
    {
        var roles = new List<OrganizationCustomRole>
        {
            new(OrgId, "admin-custom", OrganizationBaseRole.Admin, "Custom admin role"),
            new(OrgId, "teacher-advanced", OrganizationBaseRole.Teacher, null),
        };
        _roleRepoMock
            .Setup(r => r.GetByOrganizationAsync(OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var result = await _sut.ListAsync(OrgId, CancellationToken.None);

        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenNonMember_WhenListAsync_ThenThrowsForbiddenException()
    {
        _authServiceMock
            .Setup(a => a.GetCurrentMemberAsync(OrgId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException("Не является участником организации."));

        var act = () => _sut.ListAsync(OrgId, CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }
}
