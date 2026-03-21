namespace Edvantix.Organizations.Features.GroupMembership.AddStudentToGroup;

/// <summary>Validates <see cref="AddStudentToGroupCommand"/> before the handler is invoked.</summary>
internal sealed class AddStudentToGroupCommandValidator : AbstractValidator<AddStudentToGroupCommand>
{
    public AddStudentToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.ProfileId).NotEmpty();
    }
}
