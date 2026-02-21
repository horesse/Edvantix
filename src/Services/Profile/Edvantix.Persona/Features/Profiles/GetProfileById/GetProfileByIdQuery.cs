namespace Edvantix.Persona.Features.Profiles.GetProfileById;

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

        return provider.Map<ProfileModel, Domain.AggregatesModel.ProfileAggregate.Profile>(profile);
    }
}
