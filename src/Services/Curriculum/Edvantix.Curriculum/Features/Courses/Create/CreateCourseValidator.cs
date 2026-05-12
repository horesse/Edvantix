using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses.Create;

internal sealed class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(32)
            .Matches(@"^[A-Za-z0-9\-]+$")
            .WithMessage("Код курса может содержать только латинские буквы, цифры и дефис.");

        RuleFor(x => x.Name).NotEmpty().MaximumLength(512);

        RuleFor(x => x.Level).NotEmpty().MaximumLength(16);

        RuleFor(x => x.DurationWeeks).GreaterThan((short)0);

        RuleFor(x => x.OwnerMemberId).NotEmpty();

        RuleFor(x => x.Description).MaximumLength(4096).When(x => x.Description is not null);

        RuleFor(x => x.Subject)
            .IsInEnum()
            .WithMessage(
                $"Допустимые предметы: {string.Join(", ", Enum.GetNames<CourseSubject>())}"
            );
    }
}
