using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.GetProfileById;

/// <summary>
/// Запрос для получения профиля по ID
/// </summary>
public sealed record GetProfileByIdQuery(long ProfileId) : IRequest<ProfileModel>;

/// <summary>
/// Обработчик запроса получения профиля по ID
/// </summary>
public sealed class GetProfileByIdQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetProfileByIdQuery, ProfileModel>
{
    public async Task<ProfileModel> Handle(
        GetProfileByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var profile = await profileRepo.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile is null)
            throw new NotFoundException($"Профиль с ID {request.ProfileId} не найден.");

        return provider.Map<ProfileModel, Profile>(profile);
    }
}
