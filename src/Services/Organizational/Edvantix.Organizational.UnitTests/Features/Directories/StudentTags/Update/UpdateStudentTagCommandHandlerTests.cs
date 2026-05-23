using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Update;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Update;

public sealed class UpdateStudentTagCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentTag, StudentTagDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly UpdateStudentTagCommandHandler _handler;

    public UpdateStudentTagCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(
            _tenantMock.Object,
            _claimsMock.Object,
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task GivenExistingTag_WhenUpdating_ThenShouldUpdateAndReturnDto()
    {
        var tag = CreateTag(_orgId);
        var expectedDto = CreateDto(tag.Id);
        var command = new UpdateStudentTagCommand(tag.Id, "Premium", "#0000FF", 1);
        _repoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map(tag)).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        tag.Name.ShouldBe("Premium");
        tag.Color.ShouldBe("#0000FF");
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenTagNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((StudentTag?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateStudentTagCommand(id, "VIP", "#FF5733"), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenTagFromDifferentOrganization_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var tag = CreateTag(Guid.CreateVersion7());
        _repoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tag);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateStudentTagCommand(tag.Id, "VIP", "#FF5733"), CancellationToken.None).AsTask()
        );
    }

    private static StudentTag CreateTag(Guid orgId) =>
        new(orgId, "VIP", "#FF5733");

    private static StudentTagDto CreateDto(Guid id) =>
        new(id, "Premium", "#0000FF", false, 1, Guid.CreateVersion7(), DateTime.UtcNow, null, null, null);
}
