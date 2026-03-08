using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>Причина отказа в авторизации: у пользователя нет зарегистрированного профиля.</summary>
public sealed class ProfileNotRegisteredFailureReason(IAuthorizationHandler handler)
    : AuthorizationFailureReason(handler, "Profile not registered in Persona service.");
