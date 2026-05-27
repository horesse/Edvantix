using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Organizational.Features.Directories.LessonTypes.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.LessonTypes.Reorder;

public sealed class ReorderLessonTypesCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ReorderLessonTypesCommandHandler _handler;

    public ReorderLessonTypesCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreeLessonTypes_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var lt1 = CreateLessonType();
        var lt2 = CreateLessonType();
        var lt3 = CreateLessonType();
        SetupList([lt1, lt2, lt3]);

        await _handler.Handle(
            new ReorderLessonTypesCommand([lt3.Id, lt1.Id, lt2.Id]),
            CancellationToken.None
        );

        lt3.Order.ShouldBe(0);
        lt1.Order.ShouldBe(1);
        lt2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var lt = CreateLessonType();
        SetupList([lt]);

        await _handler.Handle(new ReorderLessonTypesCommand([lt.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var lt = CreateLessonType();
        SetupList([lt]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderLessonTypesCommand([unknownId, lt.Id]),
            CancellationToken.None
        );

        lt.Order.ShouldBe(1);
    }

    private LessonType CreateLessonType()
    {
        var lt = new LessonType(
            _orgId,
            "Урок",
            "LESSON",
            defaultDurationMinutes: 45,
            color: "#6366F1",
            icon: null
        );
        lt.Id = Guid.CreateVersion7();
        return lt;
    }

    private void SetupList(IReadOnlyList<LessonType> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<LessonType>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);
}
