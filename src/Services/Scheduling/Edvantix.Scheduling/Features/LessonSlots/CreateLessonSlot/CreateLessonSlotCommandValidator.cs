namespace Edvantix.Scheduling.Features.LessonSlots.CreateLessonSlot;

/// <summary>Validates <see cref="CreateLessonSlotCommand"/> before the handler is invoked.</summary>
internal sealed class CreateLessonSlotCommandValidator : AbstractValidator<CreateLessonSlotCommand>
{
    public CreateLessonSlotCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("GroupId must not be empty.");

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
