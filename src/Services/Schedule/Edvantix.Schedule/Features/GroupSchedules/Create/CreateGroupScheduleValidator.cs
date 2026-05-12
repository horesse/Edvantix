using Edvantix.Schedule.Domain.Enums;

namespace Edvantix.Schedule.Features.GroupSchedules.Create;

internal sealed class CreateGroupScheduleValidator : AbstractValidator<CreateGroupScheduleCommand>
{
    public CreateGroupScheduleValidator()
    {
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.OrganizationId).NotEmpty();
        RuleFor(c => c.LessonDurationMinutes).GreaterThan((short)0);

        RuleFor(c => c.EndDate)
            .NotNull()
            .Must((cmd, date) => date > cmd.StartDate)
            .WithMessage("Дата окончания должна быть позже даты начала.")
            .When(c => c.EndMode == EndMode.Date);

        RuleFor(c => c.LessonCount)
            .NotNull()
            .GreaterThan((short)0)
            .When(c => c.EndMode == EndMode.Count);

        RuleFor(c => c.BiweeklyParity)
            .NotNull()
            .Must(p => p == 0 || p == 1)
            .WithMessage("BiweeklyParity должен быть 0 или 1.")
            .When(c => c.Recurrence == RecurrenceType.Biweekly);

        RuleForEach(c => c.Slots).ChildRules(slot =>
        {
            slot.RuleFor(s => s.Weekday).InclusiveBetween(0, 6);
            slot.RuleFor(s => s.StartMinutes).InclusiveBetween(0, 1439);
        });
    }
}
