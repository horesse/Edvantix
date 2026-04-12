using System.Security.Claims;

namespace Edvantix.Chassis.Security.TokenExchange;

public interface ITokenExchange
{
    /// <summary>
    /// Обменивает токен субъекта на новый токен с использованием grant token-exchange от Keycloak.
    /// </summary>
    /// <param name="claimsPrincipal">Принципал утверждений, содержащий токен субъекта.</param>
    /// <param name="audience">Необязательная запрашиваемая аудитория для обменянного токена.</param>
    /// <param name="scope">Необязательная область действия для обменянного токена.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обменянный токен доступа.</returns>
    Task<string> ExchangeAsync(
        ClaimsPrincipal claimsPrincipal,
        string? audience = null,
        string? scope = null,
        CancellationToken cancellationToken = default
    );
}
