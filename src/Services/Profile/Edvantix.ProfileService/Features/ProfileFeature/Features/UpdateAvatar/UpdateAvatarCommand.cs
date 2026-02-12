using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Infrastructure.Blob;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.UpdateAvatar;

/// <summary>
/// Команда для обновления аватара текущего пользователя
/// </summary>
public sealed class UpdateAvatarCommand : IRequest<Unit>
{
    public IFormFile Image { get; set; } = null!;
}

/// <summary>
/// Обработчик команды обновления аватара текущего пользователя
/// </summary>
public sealed class UpdateAvatarCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateAvatarCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var userGuid = provider.GetUserId();
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var blobService = provider.GetRequiredService<IBlobService>();

            // Удаляем старый аватар из хранилища, если он существует
            if (profile.Avatar is not null)
                await blobService.DeleteFileAsync(profile.Avatar, cancellationToken);

            var url = await blobService.UploadFileAsync(request.Image, cancellationToken);
            profile.UploadAvatar(url);

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
