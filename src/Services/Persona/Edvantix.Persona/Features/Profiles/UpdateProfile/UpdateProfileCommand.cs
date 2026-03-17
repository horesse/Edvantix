using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateProfile;

/// <summary>PATCH /v1/profile — единый метод обновления профиля.</summary>
public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate,
    string? Bio,
    List<ContactRequest> Contacts,
    List<EmploymentHistoryRequest> EmploymentHistories,
    List<EducationRequest> Educations,
    List<string> Skills
) : ICommand<ProfileDetailsModel>;

public sealed class UpdateProfileCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateProfileCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateProfileCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var skillRepo = provider.GetRequiredService<ISkillRepository>();

        var spec = new ProfileSpecification(profileId, withDetails: true, asTracking: true);
        var profile =
            await profileRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        // Обновляем личные данные
        profile.UpdatePersonalInfo(command.BirthDate);
        profile.UpdateFullName(command.FirstName, command.LastName, command.MiddleName);
        profile.UpdateBio(command.Bio);

        // Заменяем коллекции контактов, опыта и образования
        profile.ReplaceContacts(
            command.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );
        profile.ReplaceEmploymentHistories(
            command.EmploymentHistories.Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );
        profile.ReplaceEducations(
            command.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        // Разрешаем имена навыков в ID каталога (find-or-create), затем заменяем список
        var skillIds = await ResolveSkillIdsAsync(command.Skills, skillRepo, cancellationToken);
        profile.ReplaceSkills(skillIds);

        await profileRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }

    /// <summary>
    /// Разрешает список названий навыков в ID из глобального каталога.
    /// Навыки, которых ещё нет в каталоге, создаются автоматически.
    /// Дубликаты (без учёта регистра) схлопываются в одну запись.
    /// </summary>
    private static async Task<List<Guid>> ResolveSkillIdsAsync(
        List<string> names,
        ISkillRepository skillRepo,
        CancellationToken cancellationToken
    )
    {
        var skillIds = new List<Guid>(names.Count);

        // Дедупликация по нормализованному имени (регистронезависимо) до обращений к БД
        var uniqueNames = names
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in uniqueNames)
        {
            var spec = new SkillSpecification(name);

            var skill =
                await skillRepo.FindAsync(spec, cancellationToken)
                ?? await skillRepo.AddAsync(new Skill(name), cancellationToken);

            skillIds.Add(skill.Id);
        }

        return skillIds;
    }
}
