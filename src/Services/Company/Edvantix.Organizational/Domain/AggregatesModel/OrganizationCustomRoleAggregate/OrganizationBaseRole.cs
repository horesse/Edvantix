namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Базовая роль организации, определяющая уровень доступа.
/// Значения перечислены в порядке убывания привилегий.
/// </summary>
public enum OrganizationBaseRole
{
    Owner,
    Admin,
    Manager,
    Teacher,
    Student,
}
