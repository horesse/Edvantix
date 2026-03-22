namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

public sealed class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarValidator()
    {
        RuleFor(x => x.Avatar).ApplyImageRules();
    }
}
