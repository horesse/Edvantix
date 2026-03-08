using System.Security.Claims;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
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

    /// <summary>
    /// Извлекает логин (preferred_username) текущего пользователя из claims.
    /// </summary>
    public static string GetUserLogin(this IServiceProvider provider)
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>() ?? throw new Exception("Вы не авторизованы.");

        var login = claimsPrincipal.GetClaimValue(KeycloakClaimTypes.PreferredUsername);

        return Guard.Against.NotAuthenticated(login);
    }

    /// <summary>
    /// Извлекает <c>profile_id</c> из claims.
    /// Клейм появляется только после регистрации профиля в Persona-сервисе.
    /// </summary>
    public static Guid? TryGetProfileId(this IServiceProvider provider)
    {
        var claimsPrincipal = provider.GetService<ClaimsPrincipal>();
        var value = claimsPrincipal?.GetClaimValue(KeycloakClaimTypes.ProfileId);

        return Guid.TryParse(value, out var id) ? id : null;
    }
}
