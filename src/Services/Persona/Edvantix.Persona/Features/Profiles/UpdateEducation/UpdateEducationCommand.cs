using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateEducation;

/// <summary>PATCH /v1/profile/education — обновить образование профиля.</summary>
public sealed record UpdateEducationCommand(List<EducationRequest> Educations)
    : IRequest<ProfileDetailsModel>;

public sealed class UpdateEducationCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateEducationCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateEducationCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.ReplaceEducations(
            command.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
