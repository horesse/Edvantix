using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace Edvantix.Chassis.Security.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal claimsPrincipal)
    {
        /// <summary>
        /// Получает все значения role-клеймов текущего принципала.
        /// </summary>
        /// <returns>
        /// Массив со значениями ролей.
        /// </returns>
        public string[] GetRoles()
        {
            return [.. claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value)];
        }

        /// <summary>
        /// Определяет, есть ли у текущего принципала указанная роль.
        /// </summary>
        /// <param name="role">
        /// Значение роли для проверки.
        /// </param>
        /// <returns>
        /// <see langword="true" />, если найдено соответствующее role-клеймо; иначе <see langword="false" />.
        /// </returns>
        public bool HasRole(string role)
        {
            return claimsPrincipal.FindAll(ClaimTypes.Role).Any(c => c.Value == role);
        }

        /// <summary>
        /// Получает значение первого клейма, соответствующего указанному типу.
        /// </summary>
        /// <param name="claimType">
        /// Тип клейма для поиска.
        /// </param>
        /// <returns>
        /// Значение клейма, если найдено совпадение; иначе <see langword="null" />.
        /// </returns>
        public string? GetClaimValue(string claimType)
        {
            var claim = claimsPrincipal.FindFirst(claimType);
            return claim?.Value;
        }

        /// <summary>
        /// Пытается прочитать JSON-клейм и разобрать его как <see cref="JsonNode" />.
        /// </summary>
        /// <param name="claimType">
        /// Тип клейма для поиска.
        /// </param>
        /// <param name="claimJson">
        /// При возврате метода содержит разобранное JSON-значение, если клейм существует и содержит корректный JSON;
        /// иначе <see langword="null" />. Этот параметр считается неинициализированным.
        /// </param>
        /// <returns>
        /// <see langword="true" />, если JSON-клейм найден и успешно разобран; иначе <see langword="false" />.
        /// </returns>
        /// <exception cref="System.Text.Json.JsonException">
        /// Значение клейма помечено как JSON, но содержит некорректные JSON-данные.
        /// </exception>
        public bool TryGetJsonClaim(string claimType, [NotNullWhen(true)] out JsonNode? claimJson)
        {
            var candidateClaim = claimsPrincipal.FindFirst(claimType);

            claimJson =
                candidateClaim is not null
                && string.Equals(
                    candidateClaim.ValueType,
                    "JSON",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? JsonNode.Parse(candidateClaim.Value)
                    : null;

            return claimJson is not null;
        }
    }
}
