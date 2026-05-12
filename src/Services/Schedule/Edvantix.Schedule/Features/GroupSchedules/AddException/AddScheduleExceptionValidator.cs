namespace Edvantix.Schedule.Features.GroupSchedules.AddException;

internal sealed class AddScheduleExceptionValidator : AbstractValidator<AddScheduleExceptionCommand>
{
    public AddScheduleExceptionValidator()
    {
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.ExceptionDate).NotEmpty();
        RuleFor(c => c.Reason).MaximumLength(500).When(c => c.Reason is not null);
    }
}
