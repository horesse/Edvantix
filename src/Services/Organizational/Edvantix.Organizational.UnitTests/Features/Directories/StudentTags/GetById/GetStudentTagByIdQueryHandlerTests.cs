using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.GetById;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.GetById;

public sealed class GetStudentTagByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentTag, StudentTagDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetStudentTagByIdQueryHandler _handler;

    public GetStudentTagByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingTag_WhenGettingById_ThenShouldReturnDto()
    {
        var tag = new StudentTag(_orgId, "VIP", "#FF5733");
        var expectedDto = CreateDto(tag.Id);
        _repoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map(tag)).Returns(expectedDto);

        var result = await _handler.Handle(new GetStudentTagByIdQuery(tag.Id), CancellationToken.None);

        result.ShouldBe(expectedDto);
    }

    [Test]
    public async Task GivenTagNotFound_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((StudentTag?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetStudentTagByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenTagFromDifferentOrganization_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var tag = new StudentTag(Guid.CreateVersion7(), "VIP", "#FF5733");
        _repoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tag);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetStudentTagByIdQuery(tag.Id), CancellationToken.None).AsTask()
        );
    }

    private static StudentTagDto CreateDto(Guid id) =>
        new(id, "VIP", "#FF5733", false, 0, Guid.CreateVersion7(), DateTime.UtcNow, null, null, null);
}
