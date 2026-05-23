using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags.Archive;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Archive;

public sealed class ArchiveStudentTagCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ArchiveStudentTagCommandHandler _handler;

    public ArchiveStudentTagCommandHandlerTests()
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
    public async Task GivenActiveTag_WhenArchiving_ThenShouldSetIsArchivedAndSave()
    {
        var tag = CreateTag(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        await _handler.Handle(new ArchiveStudentTagCommand(tag.Id), CancellationToken.None);

        tag.IsArchived.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAlreadyArchivedTag_WhenArchiving_ThenShouldBeIdempotent()
    {
        var tag = CreateTag(_orgId);
        tag.Archive(_profileId);
        _repoMock
            .Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        await _handler.Handle(new ArchiveStudentTagCommand(tag.Id), CancellationToken.None);

        tag.IsArchived.ShouldBeTrue();
    }

    [Test]
    public async Task GivenTagNotFound_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var tagId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentTag?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveStudentTagCommand(tagId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenTagFromDifferentOrganization_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var tag = CreateTag(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveStudentTagCommand(tag.Id), CancellationToken.None).AsTask()
        );
    }

    private static StudentTag CreateTag(Guid orgId) => new(orgId, "VIP", "#FF5733");
}
