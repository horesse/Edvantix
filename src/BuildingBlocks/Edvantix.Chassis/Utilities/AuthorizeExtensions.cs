using System.Security.Claims;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Utilities;

public static class AuthorizeExtensions
{
    public static Guid GetUserId(this IServiceProvider provider)
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>() ?? throw new Exception("Вы не авторизованы.");

        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);

        var userGuid = Guid.Parse(userId);

        return userGuid;
    }
}
