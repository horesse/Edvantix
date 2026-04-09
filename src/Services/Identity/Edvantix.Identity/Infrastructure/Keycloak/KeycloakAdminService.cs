using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Edvantix.Chassis.Security.Settings;

namespace Edvantix.Identity.Infrastructure.Keycloak;

/// <summary>
/// Реализация <see cref="IKeycloakAdminService"/>.
/// Получает токен сервисного аккаунта через client_credentials flow,
/// затем обновляет атрибуты пользователя через Admin REST API.
/// </summary>
public sealed class KeycloakAdminService(
    IHttpClientFactory httpClientFactory,
    IdentityOptions identityOptions
) : IKeycloakAdminService
{
    /// <inheritdoc />
    public async Task SetProfileIdAsync(
        Guid accountId,
        Guid profileId,
        CancellationToken cancellationToken = default
    )
    {
        var token = await GetServiceAccountTokenAsync(cancellationToken);
        await UpdateUserAttributesAsync(accountId, profileId, token, cancellationToken);
    }

    /// <summary>
    /// Получает токен сервисного аккаунта через client_credentials.
    /// Identity client должен иметь роль manage-users в realm-management.
    /// </summary>
    private async Task<string> GetServiceAccountTokenAsync(CancellationToken cancellationToken)
    {
        using var client = httpClientFactory.CreateClient(Components.KeyCloak);

        var tokenEndpoint = $"realms/{identityOptions.Realm}/protocol/openid-connect/token";

        using var content = new FormUrlEncodedContent([
            new("grant_type", "client_credentials"),
            new("client_id", identityOptions.ClientId),
            new("client_secret", identityOptions.ClientSecret),
        ]);

        var response = await client.PostAsync(tokenEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException(
                "Keycloak не вернул access_token для сервисного аккаунта."
            );
    }

    /// <inheritdoc />
    public async Task DisableUserAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    )
    {
        await SetUserEnabledAsync(accountId, enabled: false, cancellationToken);
    }

    /// <inheritdoc />
    public async Task EnableUserAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        await SetUserEnabledAsync(accountId, enabled: true, cancellationToken);
    }

    /// <summary>Устанавливает флаг enabled для учётной записи Keycloak через Admin API.</summary>
    private async Task SetUserEnabledAsync(
        Guid accountId,
        bool enabled,
        CancellationToken cancellationToken
    )
    {
        var token = await GetServiceAccountTokenAsync(cancellationToken);

        using var client = httpClientFactory.CreateClient(Components.KeyCloak);

        var userEndpoint = $"admin/realms/{identityOptions.Realm}/users/{accountId}";

        var payload = JsonSerializer.Serialize(new { enabled });

        using var request = new HttpRequestMessage(HttpMethod.Put, userEndpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Выполняет PUT-запрос к Admin API, чтобы добавить атрибут profileId.
    /// </summary>
    private async Task UpdateUserAttributesAsync(
        Guid accountId,
        Guid profileId,
        string token,
        CancellationToken cancellationToken
    )
    {
        using var client = httpClientFactory.CreateClient(Components.KeyCloak);

        var userEndpoint = $"admin/realms/{identityOptions.Realm}/users/{accountId}";

        var payload = JsonSerializer.Serialize(
            new
            {
                attributes = new Dictionary<string, string[]>
                {
                    // Keycloak хранит атрибуты как массивы строк
                    ["profileId"] = [profileId.ToString()],
                },
            }
        );

        using var request = new HttpRequestMessage(HttpMethod.Put, userEndpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
