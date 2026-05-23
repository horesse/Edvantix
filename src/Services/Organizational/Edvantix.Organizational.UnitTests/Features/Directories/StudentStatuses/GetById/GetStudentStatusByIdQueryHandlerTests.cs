using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses;
using Edvantix.Organizational.Features.Directories.StudentStatuses.GetById;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.GetById;

public sealed class GetStudentStatusByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentStatus, StudentStatusDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetStudentStatusByIdQueryHandler _handler;

    public GetStudentStatusByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingStatus_WhenQuerying_ThenShouldReturnDto()
    {
        var status = new StudentStatus(_orgId, "Активный", "ACTIVE", StudentStatusTone.Active);
        var expectedDto = CreateDto(status.Id);
        _repoMock
            .Setup(r => r.GetByIdAsync(status.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);
        _mapperMock.Setup(m => m.Map(status)).Returns(expectedDto);

        var result = await _handler.Handle(
            new GetStudentStatusByIdQuery(status.Id),
            CancellationToken.None
        );

        result.ShouldBe(expectedDto);
    }

    [Test]
    public async Task GivenNonExistentStatus_WhenQuerying_ThenShouldThrowNotFound()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentStatus?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new GetStudentStatusByIdQuery(Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenStatusOfAnotherOrg_WhenQuerying_ThenShouldThrowNotFound()
    {
        var status = new StudentStatus(
            Guid.CreateVersion7(),
            "Другой",
            "OTHER",
            StudentStatusTone.Neutral
        );
        _repoMock
            .Setup(r => r.GetByIdAsync(status.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetStudentStatusByIdQuery(status.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private static StudentStatusDto CreateDto(Guid id) =>
        new(
            id,
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            false,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
