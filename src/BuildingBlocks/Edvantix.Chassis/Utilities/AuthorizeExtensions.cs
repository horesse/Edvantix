using System.Security.Claims;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Guards;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Utilities;

public static class AuthorizeExtensions
{
    extension(IServiceProvider provider)
    {
        public Guid GetUserId()
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
        public string GetUserLogin()
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
        public Guid? TryGetProfileId()
        {
            var claimsPrincipal = provider.GetService<ClaimsPrincipal>();
            var value = claimsPrincipal?.GetClaimValue(KeycloakClaimTypes.ProfileId);

            return Guid.TryParse(value, out var id) ? id : null;
        }

        public Guid GetProfileIdOrError()
        {
            return provider.TryGetProfileId() ?? throw new ForbiddenException("У Вас нет профиля.");
        }
    }
}
