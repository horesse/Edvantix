namespace Edvantix.Curriculum.Features.Lessons.Update;

internal sealed class UpdateLessonValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(512);
        RuleFor(x => x.Minutes).GreaterThan((short)0);
        RuleFor(x => x.Type).IsInEnum();
        RuleForEach(x => x.Objectives).MaximumLength(512);
    }
}
