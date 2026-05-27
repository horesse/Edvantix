using Edvantix.Organizational.Features.Directories.StudentTags.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Reorder;

public sealed class ReorderStudentTagsCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ReorderStudentTagsCommandHandler _handler;

    public ReorderStudentTagsCommandHandlerTests()
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
    public async Task GivenThreeTags_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var t1 = CreateTag();
        var t2 = CreateTag();
        var t3 = CreateTag();
        SetupList([t1, t2, t3]);

        await _handler.Handle(
            new ReorderStudentTagsCommand([t3.Id, t1.Id, t2.Id]),
            CancellationToken.None
        );

        t3.Order.ShouldBe(0);
        t1.Order.ShouldBe(1);
        t2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var t = CreateTag();
        SetupList([t]);

        await _handler.Handle(new ReorderStudentTagsCommand([t.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var t = CreateTag();
        SetupList([t]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderStudentTagsCommand([unknownId, t.Id]),
            CancellationToken.None
        );

        t.Order.ShouldBe(1);
    }

    private StudentTag CreateTag()
    {
        var tag = new StudentTag(_orgId, "VIP", "#FF5733");
        tag.Id = Guid.CreateVersion7();
        return tag;
    }

    private void SetupList(IReadOnlyList<StudentTag> items) =>
        _repoMock
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);
}
