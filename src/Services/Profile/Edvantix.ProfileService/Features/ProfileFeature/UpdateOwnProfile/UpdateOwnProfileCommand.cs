using System.Security.Claims;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.UpdateOwnProfile;

/// <summary>
/// Команда для обновления собственного профиля
/// </summary>
public sealed record UpdateOwnProfileCommand(UpdateProfileModel Profile) : IRequest<Unit>;

/// <summary>
/// Обработчик команды обновления собственного профиля
/// </summary>
public sealed class UpdateOwnProfileCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOwnProfileCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateOwnProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>()
            ?? throw new UnauthorizedAccessException("Вы не авторизованы.");

        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);
        var userGuid = Guid.Parse(userId);

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

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
