using System.Security.Claims;
using Edvantix.Chassis.Utilities;

namespace Edvantix.Blog.Grpc.Services;

/// <summary>
/// Вспомогательные методы для работы с Profile gRPC-сервисом из обработчиков команд.
/// </summary>
public static class ProfileExtensions
{
    /// <summary>
    /// Получает идентификатор профиля текущего авторизованного пользователя
    /// через gRPC-сервис Profile.
    /// </summary>
    /// <param name="provider">Провайдер сервисов.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public static async Task<long> GetProfileId(
        this IServiceProvider provider,
        CancellationToken cancellationToken
    )
    {
        var userId = provider.GetUserId();

        var profileService = provider.GetRequiredService<IProfileService>();
        return await profileService.GetProfileIdByAccountId(userId, cancellationToken);
    }

    /// <summary>
    /// Пытается получить идентификатор профиля текущего пользователя.
    /// Возвращает <c>null</c>, если пользователь не авторизован.
    /// </summary>
    /// <param name="provider">Провайдер сервисов.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public static async Task<long?> TryGetProfileId(
        this IServiceProvider provider,
        CancellationToken cancellationToken
    )
    {
        var claimsPrincipal = provider.GetService<ClaimsPrincipal>();

        if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            return null;

        return await provider.GetProfileId(cancellationToken);
    }
}
