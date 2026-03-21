using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;
using Edvantix.Scheduling.Features.Attendance.MarkAttendance;
using Shouldly;

namespace Edvantix.Scheduling.UnitTests.Features;

/// <summary>
/// Unit tests for <see cref="MarkAttendanceCommandHandler"/>.
/// Verifies upsert logic: new record is added, existing record has status updated (not re-added).
/// </summary>
public sealed class MarkAttendanceCommandHandlerTests
{
    private readonly Mock<IAttendanceRecordRepository> _repositoryMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public MarkAttendanceCommandHandlerTests()
    {
        _tenantContextMock.Setup(t => t.SchoolId).Returns(Guid.NewGuid());
        _repositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Test]
    public async Task GivenNoExistingRecord_WhenMarkingAttendance_ThenNewRecordIsAdded()
    {
        var slotId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        _repositoryMock
            .Setup(r =>
                r.FindBySlotAndStudentAsync(
                    slotId,
                    studentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((AttendanceRecord?)null);

        var command = new MarkAttendanceCommand(slotId, studentId, AttendanceStatus.Present);
        var handler = new MarkAttendanceCommandHandler(_repositoryMock.Object, _tenantContextMock.Object);

        await handler.Handle(command, CancellationToken.None);

        // A new record must be added exactly once
        _repositoryMock.Verify(
            r => r.Add(It.Is<AttendanceRecord>(rec =>
                rec.LessonSlotId == slotId &&
                rec.StudentId == studentId &&
                rec.Status == AttendanceStatus.Present)),
            Times.Once
        );

        // SaveEntitiesAsync must be called exactly once
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingRecord_WhenMarkingAttendance_ThenStatusIsUpdatedNotReAdded()
    {
        var slotId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var schoolId = _tenantContextMock.Object.SchoolId;

        var existingRecord = new AttendanceRecord(
            schoolId,
            slotId,
            studentId,
            AttendanceStatus.Present
        );

        _repositoryMock
            .Setup(r =>
                r.FindBySlotAndStudentAsync(
                    slotId,
                    studentId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingRecord);

        var command = new MarkAttendanceCommand(slotId, studentId, AttendanceStatus.Absent);
        var handler = new MarkAttendanceCommandHandler(_repositoryMock.Object, _tenantContextMock.Object);

        await handler.Handle(command, CancellationToken.None);

        // Add must NOT be called — the existing record is updated via EF change tracking
        _repositoryMock.Verify(r => r.Add(It.IsAny<AttendanceRecord>()), Times.Never);

        // SaveEntitiesAsync must be called exactly once
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        // The record's status should reflect the new value
        existingRecord.Status.ShouldBe(AttendanceStatus.Absent);
    }
}
