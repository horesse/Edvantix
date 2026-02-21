namespace Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;

/// <summary>
/// Команда для обновления профиля администратором
/// </summary>
public sealed record UpdateProfileByAdminCommand(long ProfileId, ProfileModel Profile)
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
