namespace Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;

/// <summary>Команда обновления профиля администратором. Возвращает обновлённое краткое представление.</summary>
public sealed record UpdateProfileByAdminCommand(ulong ProfileId, UpdateProfileRequest Request)
    : IRequest<ProfileViewModel>;

public sealed class UpdateProfileByAdminCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateProfileByAdminCommand, ProfileViewModel>
{
    public async ValueTask<ProfileViewModel> Handle(
        UpdateProfileByAdminCommand command,
        CancellationToken ct
    )
    {
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByIdSpec(command.ProfileId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException($"Профиль с ID {command.ProfileId} не найден.");

        ApplyChanges(profile, command.Request);

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();
        return mapper.Map(profile);
    }

    private static void ApplyChanges(Profile profile, UpdateProfileRequest request)
    {
        profile.UpdatePersonalInfo(request.Gender, request.BirthDate);
        profile.UpdateFullName(request.FirstName, request.LastName, request.MiddleName);

        profile.ReplaceContacts(
            request.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );

        profile.ReplaceEducations(
            request.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        profile.ReplaceEmploymentHistories(
            request.EmploymentHistories.Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );
    }
}
