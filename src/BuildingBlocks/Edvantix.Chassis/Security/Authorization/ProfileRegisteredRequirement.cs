using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>Требует наличия claim <c>profile_id</c> в токене пользователя.</summary>
public sealed class ProfileRegisteredRequirement : IAuthorizationRequirement;
