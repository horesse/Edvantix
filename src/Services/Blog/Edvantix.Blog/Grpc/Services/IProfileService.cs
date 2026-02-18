namespace Edvantix.Blog.Grpc.Services;

/// <summary>
/// Клиент для получения данных профилей через gRPC-сервис Profile.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Возвращает идентификатор профиля по идентификатору аккаунта Keycloak.
    /// </summary>
    /// <param name="accountId">UUID аккаунта пользователя в Keycloak.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<long> GetProfileIdByAccountId(Guid accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает данные профиля по его идентификатору.
    /// </summary>
    /// <param name="profileId">Идентификатор профиля.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<AuthorInfo?> GetAuthorById(long profileId, CancellationToken cancellationToken);
}

/// <summary>
/// Краткая информация об авторе поста, полученная из сервиса Profile.
/// </summary>
public sealed record AuthorInfo(long Id, string FullName, string FirstName, string LastName);
