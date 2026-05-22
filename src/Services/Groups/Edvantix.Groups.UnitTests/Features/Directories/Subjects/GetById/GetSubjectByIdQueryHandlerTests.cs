using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects;
using Edvantix.Groups.Features.Directories.Subjects.GetById;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.GetById;

public sealed class GetSubjectByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Mock<IMapper<Subject, SubjectDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetSubjectByIdQueryHandler _handler;

    public GetSubjectByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingSubject_WhenQuerying_ThenReturnsDto()
    {
        var subject = CreateSubject(_organizationId);
        var dto = BuildDto(subject);

        _repoMock
            .Setup(r => r.GetByIdAsync(subject.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        _mapperMock.Setup(m => m.Map(subject)).Returns(dto);

        var result = await _handler.Handle(
            new GetSubjectByIdQuery(subject.Id),
            CancellationToken.None
        );

        result.ShouldBe(dto);
    }

    [Test]
    public async Task GivenSubjectNotFound_WhenQuerying_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetSubjectByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenSubjectFromOtherOrg_WhenQuerying_ThenThrowsNotFoundException()
    {
        var subject = CreateSubject(Guid.CreateVersion7());

        _repoMock
            .Setup(r => r.GetByIdAsync(subject.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetSubjectByIdQuery(subject.Id), CancellationToken.None).AsTask()
        );
    }

    private static Subject CreateSubject(Guid orgId) =>
        new(orgId, "Математика", SubjectCode.From("MATH"), "#6366F1", null);

    private static SubjectDto BuildDto(Subject s) =>
        new(
            s.Id,
            s.Name,
            s.Code.Value,
            s.Color,
            s.Description,
            s.Order,
            s.IsArchived,
            s.CreatedAt,
            s.LastModifiedAt
        );
}
