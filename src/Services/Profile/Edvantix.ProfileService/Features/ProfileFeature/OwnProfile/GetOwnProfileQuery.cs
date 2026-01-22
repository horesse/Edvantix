using System.Security.Claims;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using FluentValidation;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.OwnProfile;

public sealed record GetOwnProfileQuery : IRequest<OwnProfileResponse>;

public sealed class GetOwnProfileQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOwnProfileQuery, OwnProfileResponse>
{
    public async Task<OwnProfileResponse> Handle(
        GetOwnProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>() ?? throw new Exception("Вы не авторизованы.");

        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);

        var userGuid = Guid.Parse(userId);

        var spec = new ProfileByAccountSpecification(userGuid);

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile == null)
            throw new NotFoundException("Профиль не найден.");

        var userName =
            claimsPrincipal.GetClaimValue(ClaimTypes.Name) ?? profile.AccountId.ToString();

        return new OwnProfileResponse(
            profile.Id.ToString(),
            profile.FullName.GetFullName(),
            userName
        );
    }
}
