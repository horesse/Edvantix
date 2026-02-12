using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.UpdateOwnProfile;

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
            var converter = provider.GetRequiredService<IConverter<ProfileModel, Profile>>();
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
