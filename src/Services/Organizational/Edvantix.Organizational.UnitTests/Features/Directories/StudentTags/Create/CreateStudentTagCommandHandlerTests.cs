using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Create;

public sealed class CreateStudentTagCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentTag, StudentTagDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly CreateStudentTagCommandHandler _handler;

    public CreateStudentTagCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentTag>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveAndReturnDto()
    {
        var expectedDto = CreateDto();
        var command = new CreateStudentTagCommand("VIP", "#FF5733");
        _mapperMock.Setup(m => m.Map(It.IsAny<StudentTag>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenTagShouldBelongToCurrentOrganization()
    {
        StudentTag? capturedTag = null;
        var command = new CreateStudentTagCommand("Premium", "#0000FF", Order: 1);
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentTag>(), It.IsAny<CancellationToken>()))
            .Callback<StudentTag, CancellationToken>((tag, _) => capturedTag = tag)
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<StudentTag>())).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedTag.ShouldNotBeNull();
        capturedTag!.OrganizationId.ShouldBe(_orgId);
        capturedTag.Name.ShouldBe("Premium");
        capturedTag.Color.ShouldBe("#0000FF");
        capturedTag.Order.ShouldBe(1);
        capturedTag.IsArchived.ShouldBeFalse();
    }

    private static StudentTagDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "VIP",
            "#FF5733",
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
