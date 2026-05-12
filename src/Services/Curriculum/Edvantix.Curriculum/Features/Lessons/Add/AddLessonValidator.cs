using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Lessons.Add;

internal sealed class AddLessonValidator : AbstractValidator<AddLessonCommand>
{
    public AddLessonValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(512);
        RuleFor(x => x.Minutes).GreaterThan((short)0);
        RuleFor(x => x.Type).IsInEnum();
        RuleForEach(x => x.Objectives).MaximumLength(512);
    }
}
