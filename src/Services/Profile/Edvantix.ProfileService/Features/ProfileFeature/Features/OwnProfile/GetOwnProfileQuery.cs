using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Infrastructure.Blob;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.OwnProfile;

public sealed record GetOwnProfileQuery : IRequest<ProfileViewModel>;

public sealed class GetOwnProfileQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOwnProfileQuery, ProfileViewModel>
{
    public async Task<ProfileViewModel> Handle(
        GetOwnProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var userGuid = provider.GetUserId();

        var spec = new ProfileByAccountSpecification(userGuid);

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        var blobService = provider.GetRequiredService<IBlobService>();

        var avatarUrl = profile.Avatar is not null
            ? blobService.GetFileSasUrl(profile.Avatar)
            : null;

        return new ProfileViewModel(
            profile.Id.ToString(),
            profile.FullName.GetFullName(),
            profile.Login,
            avatarUrl
        );
    }
}
