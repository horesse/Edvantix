namespace Edvantix.Curriculum.Features.Modules.Add;

internal sealed class AddModuleValidator : AbstractValidator<AddModuleCommand>
{
    public AddModuleValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
        RuleFor(x => x.Summary).MaximumLength(1024).When(x => x.Summary is not null);
        RuleFor(x => x.Weeks).GreaterThan((short)0);
    }
}
