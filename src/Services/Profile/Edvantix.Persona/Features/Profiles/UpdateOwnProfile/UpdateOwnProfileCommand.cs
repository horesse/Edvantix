using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>
/// Команда для обновления собственного профиля
/// </summary>
public sealed record UpdateOwnProfileCommand(ProfileModel Profile) : IRequest<Unit>;

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
        var userGuid = provider.GetUserId();
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid, true);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var converter = provider.GetRequiredService<
                IConverter<ProfileModel, Domain.AggregatesModel.ProfileAggregate.Profile>
            >();
            converter.SetProperties(request.Profile, profile);

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
