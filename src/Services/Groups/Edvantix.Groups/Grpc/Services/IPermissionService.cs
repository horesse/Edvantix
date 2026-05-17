namespace Edvantix.Groups.Grpc.Services;

/// <summary>
/// Проверяет разрешение профиля в организации через gRPC-сервис Organizational.
/// Ответы кешируются локально с коротким TTL, чтобы избежать gRPC-вызова на каждый запрос.
/// </summary>
internal interface IPermissionService
{
    /// <summary>
    /// Возвращает <see langword="true"/>, если профиль является активным участником
    /// организации и обладает указанным разрешением.
    /// </summary>
    Task<bool> CheckPermissionAsync(
        Guid organizationId,
        Guid profileId,
        string permission,
        CancellationToken cancellationToken
    );
}
