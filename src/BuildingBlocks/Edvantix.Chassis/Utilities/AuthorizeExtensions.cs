using System.Security.Claims;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Guards;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Utilities;

public static class AuthorizeExtensions
{
    /// <param name="claims">Объект <see cref="ClaimsPrincipal"/>, содержащий клеймы пользователя.</param>
    extension(ClaimsPrincipal claims)
    {
        /// <summary>
        /// Получает идентификатор текущего пользователя из клеймов.
        /// </summary>
        /// <returns>Идентификатор пользователя в виде <see cref="Guid"/>.</returns>
        /// <exception cref="Exception">
        /// Выбрасывается, если пользователь не аутентифицирован или идентификатор отсутствует/некорректен.
        /// </exception>
        public Guid GetUserId()
        {
            var sub = claims.GetClaimValue(ClaimTypes.NameIdentifier);
            var userId = Guard.Against.NotAuthenticated(sub);

            return Guid.Parse(userId);
        }

        /// <summary>
        /// Получает логин (идентификатор) текущего пользователя из клеймов.
        /// </summary>
        /// <returns>Идентификатор пользователя в виде <see cref="string"/>.</returns>
        /// <exception cref="Exception">
        /// Выбрасывается, если пользователь не аутентифицирован или значение клейма отсутствует/некорректно.
        /// </exception>
        public string GetUserLogin()
        {
            var userName = claims.GetClaimValue(KeycloakClaimTypes.PreferredUsername);
            var login = Guard.Against.NotAuthenticated(userName);

            return login;
        }

        /// <summary>
        /// Пытается получить идентификатор профиля пользователя из клеймов.
        /// </summary>
        /// <returns>
        /// Идентификатор профиля в виде <see cref="Guid"/>, если значение присутствует и корректно; иначе <c>null</c>.
        /// </returns>
        public Guid? TryGetProfileId()
        {
            var value = claims?.GetClaimValue(KeycloakClaimTypes.Profile);

            return Guid.TryParse(value, out var id) ? id : null;
        }

        public Guid GetProfileIdOrError()
        {
            return claims.TryGetProfileId() ?? throw new ForbiddenException("У Вас нет профиля.");
        }
    }
}
