using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Features.Attendance.MarkAttendance;

/// <summary>Validates <see cref="MarkAttendanceCommand"/> before the handler is invoked.</summary>
internal sealed class MarkAttendanceCommandValidator : AbstractValidator<MarkAttendanceCommand>
{
    public MarkAttendanceCommandValidator()
    {
        RuleFor(x => x.SlotId).NotEmpty().WithMessage("SlotId must not be empty.");

        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId must not be empty.");

        // Validates the enum value is defined — prevents invalid int values from being accepted
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid AttendanceStatus value.");
    }
}
