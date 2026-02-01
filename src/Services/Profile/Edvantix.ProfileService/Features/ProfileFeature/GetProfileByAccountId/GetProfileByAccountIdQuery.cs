using Edvantix.Chassis.Exceptions;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.GetProfileByAccountId;

/// <summary>
/// Запрос для получения профиля по AccountId (GUID пользователя)
/// </summary>
public sealed record GetProfileByAccountIdQuery(Guid AccountId) : IRequest<ProfileResponse>;

/// <summary>
/// Обработчик запроса получения профиля по AccountId
/// </summary>
public sealed class GetProfileByAccountIdQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetProfileByAccountIdQuery, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(
        GetProfileByAccountIdQuery request,
        CancellationToken cancellationToken
    )
    {
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(request.AccountId);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException($"Профиль для пользователя {request.AccountId} не найден.");

        var contacts = profile
            .Contacts.Where(c => !c.IsDeleted)
            .Select(c => new ContactResponse(c.Type, c.Value, c.Description))
            .ToList();

        var employmentHistories = profile
            .EmploymentHistories.Where(e => !e.IsDeleted)
            .Select(e => new EmploymentHistoryResponse(
                e.Workplace,
                e.Position,
                e.StartDate,
                e.EndDate,
                e.Description
            ))
            .ToList();

        var educations = profile
            .Educations.Where(e => !e.IsDeleted)
            .Select(e => new EducationResponse(
                e.DateStart,
                e.DateEnd,
                e.Institution,
                e.Specialty,
                e.EducationLevelId,
                e.EducationLevel?.Name
            ))
            .ToList();

        return new ProfileResponse(
            profile.Id,
            profile.AccountId,
            profile.Gender,
            profile.BirthDate,
            profile.FullName.GetFullName(),
            profile.FullName.FirstName,
            profile.FullName.LastName,
            profile.FullName.MiddleName,
            contacts,
            employmentHistories,
            educations
        );
    }
}
