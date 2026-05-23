namespace Edvantix.Organizational.Pipelines;

/// <summary>
/// Проверяет, является ли профиль активным участником организации и обладает ли
/// указанным разрешением. Содержит логику двухуровневого кеша (L1 = участник → roleId,
/// L2 = roleId → набор разрешений) и теги инвалидации <see cref="AuthorizationCacheKeys"/>.
/// </summary>
internal interface IPermissionChecker
{
    /// <summary>
    /// Выполняет проверку разрешения.
    /// </summary>
    /// <returns>
    /// <see langword="null"/> — профиль не является активным участником организации;<br/>
    /// <see langword="false"/> — участник, но разрешение не предоставлено;<br/>
    /// <see langword="true"/> — разрешение предоставлено.
    /// </returns>
    Task<bool?> CheckAsync(
        Guid organizationId,
        Guid profileId,
        string permission,
        CancellationToken cancellationToken
    );
}
