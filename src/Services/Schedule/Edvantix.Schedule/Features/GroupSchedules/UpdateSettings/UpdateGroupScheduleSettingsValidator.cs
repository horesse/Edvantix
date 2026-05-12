using Edvantix.Schedule.Domain.Enums;

namespace Edvantix.Schedule.Features.GroupSchedules.UpdateSettings;

internal sealed class UpdateGroupScheduleSettingsValidator
    : AbstractValidator<UpdateGroupScheduleSettingsCommand>
{
    public UpdateGroupScheduleSettingsValidator()
    {
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.LessonDurationMinutes).GreaterThan((short)0);

        RuleFor(c => c.EndDate)
            .NotNull()
            .WithMessage("Дата окончания обязательна при EndMode.Date.")
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

        RuleForEach(c => c.Slots)
            .ChildRules(slot =>
            {
                slot.RuleFor(s => s.Weekday).InclusiveBetween(0, 6);
                slot.RuleFor(s => s.StartMinutes).InclusiveBetween(0, 1439);
            });
    }
}
