using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Edvantix.Chassis.Security.Settings;

namespace Edvantix.Identity.Infrastructure.Keycloak;

/// <summary>
/// Реализация <see cref="IKeycloakAdminService"/>.
/// Получает токен сервисного аккаунта через client_credentials flow,
/// затем обновляет данные пользователя через Admin REST API.
/// </summary>
/// <remarks>
/// Все методы обновления используют паттерн GET → merge → PUT, поскольку
/// Keycloak Admin API при PUT полностью заменяет представление пользователя:
/// неуказанные поля (firstName, lastName, attributes и др.) обнуляются.
/// </remarks>
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
        var user = await GetUserAsync(accountId, token, cancellationToken);

        // Keycloak хранит атрибуты как массивы строк; мержим в существующий объект.
        var attributes = user["attributes"]?.AsObject() ?? new JsonObject();
        attributes["profileId"] = new JsonArray(JsonValue.Create(profileId.ToString()));
        user["attributes"] = attributes;

        await PutUserAsync(accountId, user, token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DisableUserAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    ) => await SetUserEnabledAsync(accountId, enabled: false, cancellationToken);

    /// <inheritdoc />
    public async Task EnableUserAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    ) => await SetUserEnabledAsync(accountId, enabled: true, cancellationToken);

    /// <inheritdoc />
    public async Task UpdateFullNameAsync(
        Guid accountId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default
    )
    {
        var token = await GetServiceAccountTokenAsync(cancellationToken);
        var user = await GetUserAsync(accountId, token, cancellationToken);

        user["firstName"] = firstName;
        user["lastName"] = lastName;

        await PutUserAsync(accountId, user, token, cancellationToken);
    }

    /// <summary>Устанавливает флаг enabled для учётной записи Keycloak через Admin API.</summary>
    private async Task SetUserEnabledAsync(
        Guid accountId,
        bool enabled,
        CancellationToken cancellationToken
    )
    {
        var token = await GetServiceAccountTokenAsync(cancellationToken);
        var user = await GetUserAsync(accountId, token, cancellationToken);

        user["enabled"] = enabled;

        await PutUserAsync(accountId, user, token, cancellationToken);
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

    /// <summary>
    /// Загружает полное представление пользователя из Keycloak Admin API.
    /// Используется перед каждым обновлением, чтобы не затереть остальные поля.
    /// </summary>
    private async Task<JsonObject> GetUserAsync(
        Guid accountId,
        string token,
        CancellationToken cancellationToken
    )
    {
        using var client = httpClientFactory.CreateClient(Components.KeyCloak);

        var userEndpoint = $"admin/realms/{identityOptions.Realm}/users/{accountId}";

        using var request = new HttpRequestMessage(HttpMethod.Get, userEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonNode.Parse(json)?.AsObject()
            ?? throw new InvalidOperationException(
                $"Keycloak вернул пустой ответ для пользователя {accountId}."
            );
    }

    /// <summary>
    /// Выполняет PUT-запрос с полным представлением пользователя к Admin API.
    /// </summary>
    private async Task PutUserAsync(
        Guid accountId,
        JsonObject user,
        string token,
        CancellationToken cancellationToken
    )
    {
        using var client = httpClientFactory.CreateClient(Components.KeyCloak);

        var userEndpoint = $"admin/realms/{identityOptions.Realm}/users/{accountId}";
        var payload = user.ToJsonString();

        using var request = new HttpRequestMessage(HttpMethod.Put, userEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
