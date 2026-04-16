namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationCreatedDomainEventHandlerTests
{
    private readonly Mock<IOrganizationMemberRoleRepository> _memberRoleRepoMock = new();
    private readonly Mock<IGroupRoleRepository> _groupRoleRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IPermissionRepository> _permissionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly OrganizationCreatedDomainEventHandler _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid OwnerProfileId = Guid.CreateVersion7();

    public OrganizationCreatedDomainEventHandlerTests()
    {
        _memberRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _permissionRepoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Permission>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        _handler = new(
            _memberRoleRepoMock.Object,
            _groupRoleRepoMock.Object,
            _memberRepoMock.Object,
            _permissionRepoMock.Object
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd5OrgRoles()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _memberRoleRepoMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.Is<IReadOnlyList<OrganizationMemberRole>>(roles => roles.Count == 5),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd4GroupRoles()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _groupRoleRepoMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.Is<IReadOnlyList<GroupRole>>(roles => roles.Count == 4),
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
        IReadOnlyList<OrganizationMemberRole>? capturedRoles = null;
        _memberRoleRepoMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IReadOnlyList<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<IReadOnlyList<OrganizationMemberRole>, CancellationToken>(
                (roles, _) => capturedRoles = roles
            );

        OrganizationMember? capturedMember = null;
        _memberRepoMock
            .Setup(r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()))
            .Callback<OrganizationMember, CancellationToken>((m, _) => capturedMember = m);

        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        var ownerRole = capturedRoles!.Single(r => r.Code == "owner");
        capturedMember!.OrganizationMemberRoleId.ShouldBe(ownerRole.Id);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSaveOnce()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldQueryBothPermissionFeatures()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _permissionRepoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<Permission>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllRolesBelongToOrganization()
    {
        IReadOnlyList<OrganizationMemberRole>? capturedOrgRoles = null;
        _memberRoleRepoMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IReadOnlyList<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<IReadOnlyList<OrganizationMemberRole>, CancellationToken>(
                (roles, _) => capturedOrgRoles = roles
            );

        var @event = new OrganizationCreatedDomainEvent(OrgId, OwnerProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        capturedOrgRoles!.ShouldAllBe(r => r.OrganizationId == OrgId);
    }
}
