namespace Edvantix.Scheduling.Features.LessonSlots.EditLessonSlot;

/// <summary>Validates <see cref="EditLessonSlotCommand"/> before the handler is invoked.</summary>
internal sealed class EditLessonSlotCommandValidator : AbstractValidator<EditLessonSlotCommand>
{
    public EditLessonSlotCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id must not be empty.");

        RuleFor(x => x.TeacherId).NotEmpty().WithMessage("TeacherId must not be empty.");

        RuleFor(x => x.StartTime)
            .NotEqual(default(DateTimeOffset))
            .WithMessage("StartTime must not be the default value.");

        RuleFor(x => x.EndTime)
            .NotEqual(default(DateTimeOffset))
            .WithMessage("EndTime must not be the default value.");

        // Validate time ordering: EndTime must be strictly after StartTime
        RuleFor(x => x.EndTime)
            .Must((cmd, endTime) => endTime > cmd.StartTime)
            .WithMessage("EndTime must be strictly after StartTime.");
    }
}
