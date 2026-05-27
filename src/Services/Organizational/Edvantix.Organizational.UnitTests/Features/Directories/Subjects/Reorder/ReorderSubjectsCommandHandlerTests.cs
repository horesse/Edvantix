using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Organizational.Features.Directories.Subjects.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Subjects.Reorder;

public sealed class ReorderSubjectsCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ReorderSubjectsCommandHandler _handler;

    public ReorderSubjectsCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreeSubjects_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var s1 = CreateSubject();
        var s2 = CreateSubject();
        var s3 = CreateSubject();
        SetupList([s1, s2, s3]);

        await _handler.Handle(
            new ReorderSubjectsCommand([s3.Id, s1.Id, s2.Id]),
            CancellationToken.None
        );

        s3.Order.ShouldBe(0);
        s1.Order.ShouldBe(1);
        s2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var s = CreateSubject();
        SetupList([s]);

        await _handler.Handle(new ReorderSubjectsCommand([s.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var s = CreateSubject();
        SetupList([s]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderSubjectsCommand([unknownId, s.Id]),
            CancellationToken.None
        );

        s.Order.ShouldBe(1);
    }

    private Subject CreateSubject()
    {
        var subject = new Subject(
            _orgId,
            "Математика",
            SubjectCode.From("MATH"),
            "#6366F1",
            description: null
        );
        subject.Id = Guid.CreateVersion7();
        return subject;
    }

    private void SetupList(IReadOnlyList<Subject> items) =>
        _repoMock
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);
}
