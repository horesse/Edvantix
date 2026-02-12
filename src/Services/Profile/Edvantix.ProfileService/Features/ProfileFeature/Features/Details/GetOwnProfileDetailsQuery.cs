using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Infrastructure.Blob;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.Details;

/// <summary>
/// Запрос для получения полной информации профиля текущего пользователя
/// </summary>
public sealed record GetOwnProfileDetailsQuery : IRequest<ProfileModel>;

/// <summary>
/// Обработчик запроса получения полной информации профиля
/// </summary>
public sealed class GetOwnProfileDetailsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOwnProfileDetailsQuery, ProfileModel>
{
    public async Task<ProfileModel> Handle(
        GetOwnProfileDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userGuid = provider.GetUserId();

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid, true);

        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        return provider.Map<ProfileModel, Profile>(profile);
    }
}
