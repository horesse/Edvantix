using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.GetById;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.GetById;

public sealed class GetLessonTypeByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetLessonTypeByIdQueryHandler _handler;

    public GetLessonTypeByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingId_WhenGettingById_ThenReturnsDto()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Урок", "LESSON", 45, "#3B82F6", "CalendarDays");

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        var dto = await _handler.Handle(new GetLessonTypeByIdQuery(id), CancellationToken.None);

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe("Урок");
        dto.Code.ShouldBe("LESSON");
    }

    [Test]
    public async Task GivenNonExistentId_WhenGettingById_ThenThrowsNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LessonType?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetLessonTypeByIdQuery(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenLessonTypeBelongingToOtherOrg_WhenGettingById_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        var otherOrgId = Guid.CreateVersion7();
        var lessonType = new LessonType(otherOrgId, "Урок", "LESSON", 45, "#3B82F6", null);

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetLessonTypeByIdQuery(id), CancellationToken.None).AsTask()
        );
    }
}
