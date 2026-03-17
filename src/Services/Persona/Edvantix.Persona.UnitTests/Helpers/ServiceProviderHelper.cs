using Edvantix.Chassis.Security.Keycloak;

namespace Edvantix.Persona.UnitTests.Helpers;

/// <summary>
/// Вспомогательные методы для создания мок-провайдера сервисов в тестах хендлеров,
/// использующих IServiceProvider для разрешения зависимостей и получения claims пользователя.
/// </summary>
internal static class ServiceProviderHelper
{
    /// <summary>
    /// Создаёт ClaimsPrincipal с минимальным набором клеймов для авторизации хендлеров.
    /// </summary>
    public static ClaimsPrincipal CreateClaimsPrincipal(Guid accountId, string login = "testuser")
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
                    new Claim(KeycloakClaimTypes.PreferredUsername, login),
                    new Claim(KeycloakClaimTypes.Profile, accountId.ToString()),
                ],
                "Bearer"
            )
        );
    }

    /// <summary>
    /// Регистрирует ClaimsPrincipal в мок-провайдере для работы GetUserId() / GetUserLogin().
    /// </summary>
    public static void SetupUser(
        this Mock<IServiceProvider> providerMock,
        Guid accountId,
        string login = "testuser"
    )
    {
        var principal = CreateClaimsPrincipal(accountId, login);
        providerMock.Setup(p => p.GetService(typeof(ClaimsPrincipal))).Returns(principal);
    }

    /// <summary>
    /// Регистрирует произвольный сервис в мок-провайдере через GetRequiredService{T}.
    /// </summary>
    public static void SetupService<T>(this Mock<IServiceProvider> providerMock, T service)
        where T : class
    {
        providerMock.Setup(p => p.GetService(typeof(T))).Returns(service);
    }
}
