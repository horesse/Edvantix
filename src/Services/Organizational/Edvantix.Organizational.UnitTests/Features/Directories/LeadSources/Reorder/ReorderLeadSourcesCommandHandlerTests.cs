using Edvantix.Organizational.Features.Directories.LeadSources.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Reorder;

public sealed class ReorderLeadSourcesCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ReorderLeadSourcesCommandHandler _handler;

    public ReorderLeadSourcesCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreeSources_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var s1 = CreateSource();
        var s2 = CreateSource();
        var s3 = CreateSource();
        SetupList([s1, s2, s3]);

        await _handler.Handle(
            new ReorderLeadSourcesCommand([s3.Id, s1.Id, s2.Id]),
            CancellationToken.None
        );

        s3.Order.ShouldBe(0);
        s1.Order.ShouldBe(1);
        s2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var s1 = CreateSource();
        SetupList([s1]);

        await _handler.Handle(new ReorderLeadSourcesCommand([s1.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenIdFromDifferentOrganization_WhenReordering_ThenShouldBeIgnored()
    {
        var own = CreateSource();
        SetupList([own]);
        var foreignId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderLeadSourcesCommand([foreignId, own.Id]),
            CancellationToken.None
        );

        own.Order.ShouldBe(1);
    }

    [Test]
    public async Task GivenEmptyOrderedIds_WhenReordering_ThenShouldSaveWithoutChanges()
    {
        SetupList([]);

        await _handler.Handle(new ReorderLeadSourcesCommand([]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private LeadSource CreateSource()
    {
        var source = new LeadSource(_orgId, "Источник", LeadChannel.Online, null);
        source.Id = Guid.CreateVersion7();
        return source;
    }

    private void SetupList(IReadOnlyList<LeadSource> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<LeadSource>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);
}
