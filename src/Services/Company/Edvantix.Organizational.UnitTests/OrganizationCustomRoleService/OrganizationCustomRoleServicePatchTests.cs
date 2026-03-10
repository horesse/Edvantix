using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Repository;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Infrastructure.Services;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.UnitTests.OrganizationCustomRoleService;

/// <summary>
/// Unit-тесты для метода <see cref="Edvantix.Organizational.Features.OrganizationCustomRoleFeature.OrganizationCustomRoleService.PatchAsync"/>.
/// </summary>
public sealed class OrganizationCustomRoleServicePatchTests
{
    private readonly Mock<IOrganizationCustomRoleRepository> _roleRepoMock;
    private readonly Mock<IOrganizationAuthorizationService> _authServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Edvantix.Organizational.Features.OrganizationCustomRoleFeature.OrganizationCustomRoleService _sut;

    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid RoleId = Guid.NewGuid();
    private static readonly Guid ProfileId = Guid.NewGuid();

    public OrganizationCustomRoleServicePatchTests()
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

        _sut =
            new Edvantix.Organizational.Features.OrganizationCustomRoleFeature.OrganizationCustomRoleService(
                _roleRepoMock.Object,
                _authServiceMock.Object
            );
    }

    // ─── PatchAsync ───────────────────────────────────────────────────────────────

    [Test]
    public async Task GivenValidRole_WhenPatchAsync_ThenBaseRoleAndDescriptionAreUpdated()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            "Old description"
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await _sut.PatchAsync(
            RoleId,
            OrgId,
            OrganizationBaseRole.Admin,
            "New description",
            CancellationToken.None
        );

        role.BaseRole.ShouldBe(OrganizationBaseRole.Admin);
        role.Description.ShouldBe("New description");
        // Код остаётся неизменным.
        role.Code.ShouldBe("teacher-advanced");
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidRole_WhenPatchAsyncWithNullDescription_ThenDescriptionIsCleared()
    {
        var role = new OrganizationCustomRole(
            OrgId,
            "teacher-advanced",
            OrganizationBaseRole.Teacher,
            "Old description"
        );
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await _sut.PatchAsync(
            RoleId,
            OrgId,
            OrganizationBaseRole.Teacher,
            null,
            CancellationToken.None
        );

        role.Description.ShouldBeNull();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNonExistentRole_WhenPatchAsync_ThenThrowsNotFoundException()
    {
        _roleRepoMock
            .Setup(r => r.FindByIdAsync(RoleId, OrgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationCustomRole?)null);

        var act = () =>
            _sut.PatchAsync(
                RoleId,
                OrgId,
                OrganizationBaseRole.Teacher,
                null,
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenNonOwnerUser_WhenPatchAsync_ThenThrowsForbiddenException()
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
            _sut.PatchAsync(
                RoleId,
                OrgId,
                OrganizationBaseRole.Teacher,
                "desc",
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenPatchAsync_WhenCalled_ThenAssignmentCheckIsNotPerformed()
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

        await _sut.PatchAsync(
            RoleId,
            OrgId,
            OrganizationBaseRole.Admin,
            null,
            CancellationToken.None
        );

        // PATCH не меняет код — проверка назначений не должна вызываться.
        _roleRepoMock.Verify(
            r => r.GetAssignedMembersCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
