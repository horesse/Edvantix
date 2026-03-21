using Edvantix.Chassis.Security.Tenant;
using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.UnitTests.Features;

/// <summary>
/// Wave 0 test stubs for MarkAttendanceCommandHandler.
/// The handler does not exist yet — it is created in Plan 02.
/// These stubs document the expected behavior and are skipped until the handler is implemented.
/// </summary>
public sealed class MarkAttendanceCommandHandlerTests
{
    private readonly Mock<IAttendanceRecordRepository> _repositoryMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();

    [Test]
    [Skip("Plan 02 — handler not yet implemented")]
    public async Task GivenNoExistingRecord_WhenMarkingAttendance_ThenNewRecordIsAdded()
    {
        // Stub: when no existing record, the handler should create a new AttendanceRecord
        // and call repository.Add(record) followed by UnitOfWork.SaveEntitiesAsync.
        _repositoryMock
            .Setup(r =>
                r.FindBySlotAndStudentAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((AttendanceRecord?)null);

        await Task.CompletedTask;

        // Will be completed in Plan 02 when handler is created.
    }

    [Test]
    [Skip("Plan 02 — handler not yet implemented")]
    public async Task GivenExistingRecord_WhenMarkingAttendance_ThenStatusIsUpdatedNotReAdded()
    {
        // Stub: when an existing record is found, the handler should call record.UpdateStatus(newStatus)
        // and NOT call repository.Add again. Only SaveEntitiesAsync is called.
        var existingRecord = new AttendanceRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            AttendanceStatus.Present
        );

        _repositoryMock
            .Setup(r =>
                r.FindBySlotAndStudentAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingRecord);

        await Task.CompletedTask;

        // Will be completed in Plan 02 when handler is created.
    }
}
