namespace Edvantix.Persona.Features.Profiles.GetProfileByAccountId;

/// <summary>
/// Запрос для получения профиля по AccountId (GUID пользователя)
/// </summary>
public sealed record GetProfileByAccountIdQuery(Guid AccountId) : IRequest<ProfileModel>;

/// <summary>
/// Обработчик запроса получения профиля по AccountId
/// </summary>
public sealed class GetProfileByAccountIdQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetProfileByAccountIdQuery, ProfileModel>
{
    public async Task<ProfileModel> Handle(
        GetProfileByAccountIdQuery request,
        CancellationToken cancellationToken
    )
    {
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(request.AccountId);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException($"Профиль для пользователя {request.AccountId} не найден.");

        return provider.Map<ProfileModel, Domain.AggregatesModel.ProfileAggregate.Profile>(profile);
    }
}
