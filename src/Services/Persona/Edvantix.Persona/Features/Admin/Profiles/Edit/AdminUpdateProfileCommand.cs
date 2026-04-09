using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Profiles.Update;

namespace Edvantix.Persona.Features.Admin.Profiles.Edit;

public sealed record AdminUpdateProfileRequest(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate,
    string? Bio,
    List<ContactRequest> Contacts,
    List<EmploymentHistoryRequest> EmploymentHistories,
    List<EducationRequest> Educations,
    List<string> Skills,
    string Reason
);

public sealed record AdminUpdateProfileCommand(
    Guid ProfileId,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate,
    string? Bio,
    List<ContactRequest> Contacts,
    List<EmploymentHistoryRequest> EmploymentHistories,
    List<EducationRequest> Educations,
    List<string> Skills,
    string Reason
) : ICommand;

public sealed class AdminUpdateProfileCommandHandler(
    IProfileRepository repository,
    ISkillRepository skillRepository,
    IBus bus,
    ILogger<AdminUpdateProfileCommandHandler> logger
) : ICommandHandler<AdminUpdateProfileCommand>
{
    public async ValueTask<Unit> Handle(
        AdminUpdateProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var spec = ProfileSpecification.ForWrite(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        // Обновляем личные данные
        profile.UpdateFullName(request.FirstName, request.LastName, request.MiddleName);
        profile.UpdatePersonalInfo(request.BirthDate);
        profile.UpdateBio(request.Bio);

        // Заменяем коллекции
        profile.ReplaceContacts(
            request.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
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
        profile.ReplaceEducations(
            request.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        // Разрешаем имена навыков в ID каталога (find-or-create)
        var skillIds = await ResolveSkillIdsAsync(request.Skills, cancellationToken);
        profile.ReplaceSkills(skillIds);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // Отправляем уведомление пользователю об изменении профиля
        var notification = new SendInAppNotificationIntegrationEvent(
            profile.AccountId,
            NotificationType.Warning,
            "Ваш профиль был изменён администратором",
            $"Администратор внёс изменения в ваш профиль. Причина: {request.Reason}"
        );

        await bus.Publish(notification, cancellationToken);

        logger.LogInformation(
            "Профиль {ProfileId} (аккаунт {AccountId}) обновлён администратором. Причина: {Reason}",
            request.ProfileId,
            profile.AccountId,
            request.Reason
        );

        return Unit.Value;
    }

    private async Task<List<Guid>> ResolveSkillIdsAsync(
        List<string> names,
        CancellationToken cancellationToken
    )
    {
        var skillIds = new List<Guid>(names.Count);

        var uniqueNames = names
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in uniqueNames)
        {
            var skillSpec = new SkillSpecification(name);

            var skill =
                await skillRepository.FindAsync(skillSpec, cancellationToken)
                ?? await skillRepository.AddAsync(new Skill(name), cancellationToken);

            skillIds.Add(skill.Id);
        }

        return skillIds;
    }
}
