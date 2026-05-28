using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class SeedRolesAndOwnerTests
{
    private readonly Mock<IOrganizationRoleRepository> _roleRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IPermissionRepository> _permissionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly SeedRolesAndOwner _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid OwnerProfileId = Guid.CreateVersion7();

    public SeedRolesAndOwnerTests()
    {
        _memberRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _permissionRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _handler = new(
            _roleRepoMock.Object,
            _memberRepoMock.Object,
            _permissionRepoMock.Object
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd7OrgRoles()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _roleRepoMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.Is<IReadOnlyList<OrganizationRole>>(roles => roles.Count == 7),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAddOwnerMemberWithCorrectData()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _memberRepoMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<OrganizationMember>(m =>
                        m.OrganizationId == OrgId && m.ProfileId == OwnerProfileId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAssignOwnerRoleToMember()
    {
        IReadOnlyList<OrganizationRole>? capturedRoles = null;
        _roleRepoMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IReadOnlyList<OrganizationRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<IReadOnlyList<OrganizationRole>, CancellationToken>(
                (roles, _) => capturedRoles = roles
            );

        OrganizationMember? capturedMember = null;
        _memberRepoMock
            .Setup(r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()))
            .Callback<OrganizationMember, CancellationToken>((m, _) => capturedMember = m);

        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        var ownerRole = capturedRoles!.Single(r => r.IsSystem);
        capturedMember!.OrganizationRoleId.ShouldBe(ownerRole.Id);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSaveOnce()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldLoadAllPermissions()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _permissionRepoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllRolesBelongToOrganization()
    {
        IReadOnlyList<OrganizationRole>? capturedRoles = null;
        _roleRepoMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IReadOnlyList<OrganizationRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<IReadOnlyList<OrganizationRole>, CancellationToken>(
                (roles, _) => capturedRoles = roles
            );

        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        capturedRoles!.ShouldAllBe(r => r.OrganizationId == OrgId);
    }
}
