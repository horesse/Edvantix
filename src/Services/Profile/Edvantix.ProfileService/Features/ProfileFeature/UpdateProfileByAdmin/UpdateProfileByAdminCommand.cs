using Edvantix.Chassis.Exceptions;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.UpdateProfileByAdmin;

/// <summary>
/// Команда для обновления профиля администратором
/// </summary>
public sealed record UpdateProfileByAdminCommand(long ProfileId, UpdateProfileModel Profile)
    : IRequest<Unit>;

/// <summary>
/// Обработчик команды обновления профиля администратором
/// </summary>
public sealed class UpdateProfileByAdminCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateProfileByAdminCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateProfileByAdminCommand request,
        CancellationToken cancellationToken
    )
    {
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var profile = await profileRepo.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile is null)
            throw new NotFoundException($"Профиль с ID {request.ProfileId} не найден.");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            // Обновляем основные поля профиля
            profile.UpdateGender(request.Profile.Gender);
            profile.UpdateBirthDate(request.Profile.BirthDate);
            profile.UpdateFullName(
                request.Profile.FirstName,
                request.Profile.LastName,
                request.Profile.MiddleName
            );

            // Обновляем контакты, если они переданы
            if (request.Profile.Contacts is not null)
            {
                var newContacts = request
                    .Profile.Contacts.Select(c =>
                        profile.CreateContact(c.Type, c.Value, c.Description)
                    )
                    .ToList();

                profile.ReplaceContacts(newContacts);
            }

            // Обновляем историю трудоустройства, если она передана
            if (request.Profile.EmploymentHistories is not null)
            {
                var newEmploymentHistories = request
                    .Profile.EmploymentHistories.Select(e =>
                        profile.CreateEmploymentHistory(
                            e.Workplace,
                            e.Position,
                            e.StartDate,
                            e.EndDate,
                            e.Description
                        )
                    )
                    .ToList();

                profile.ReplaceEmploymentHistories(newEmploymentHistories);
            }

            // Обновляем образование, если оно передано
            if (request.Profile.Educations is not null)
            {
                var newEducations = request
                    .Profile.Educations.Select(e =>
                        profile.CreateEducation(
                            e.DateStart,
                            e.Institution,
                            e.EducationLevelId,
                            e.Specialty,
                            e.DateEnd
                        )
                    )
                    .ToList();

                profile.ReplaceEducations(newEducations);
            }

            await profileRepo.UpdateAsync(profile, cancellationToken);
            await profileRepo.SaveEntitiesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
