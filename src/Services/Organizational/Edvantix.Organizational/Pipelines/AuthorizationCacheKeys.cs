using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Pipelines;

/// <summary>
/// Строители ключей и тегов кеша авторизации.
/// Префиксы строятся через <see langword="nameof"/> — при переименовании сущности ключ изменится,
/// что автоматически инвалидирует устаревшие записи в распределённом кеше.
/// </summary>
internal static class AuthorizationCacheKeys
{
    /// <summary>Ключ L1: идентификатор роли участника организации.</summary>
    public static string MemberRole(Guid organizationId, Guid profileId) =>
        $"{nameof(OrganizationMember)}:{organizationId}:{profileId}";

    /// <summary>Ключ L2 и тег: набор разрешений роли. Используется как ключ кеша и как тег инвалидации.</summary>
    public static string RolePerms(Guid roleId) => $"{nameof(OrganizationMemberRole)}:{roleId}";

    /// <summary>Тег для массовой инвалидации всего кеша авторизации организации.</summary>
    public static string OrgPermsTag(Guid organizationId) =>
        $"{nameof(OrganizationMember)}:org:{organizationId}";
}
