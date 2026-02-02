using System.Security.Claims;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Infrastructure.Blob;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.UploadAvatar;

public sealed record UploadAvatarCommand(IFormFile? Image) : IRequest<bool>;

public sealed class UploadAvatarCommandHandler(IServiceProvider provider)
    : IRequestHandler<UploadAvatarCommand, bool>
{
    public async Task<bool> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>()
            ?? throw new UnauthorizedAccessException("Вы не авторизованы.");

        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);
        var userGuid = Guid.Parse(userId);

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid);
        Profile? profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        var blobService = provider.GetRequiredService<IBlobService>();

        string? url = request.Image is null
            ? null
            : await blobService.UploadFileAsync(request.Image, cancellationToken);

        profile.UploadAvatar(url);

        await profileRepo.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
