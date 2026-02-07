using Edvantix.Constants.Other;

namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

/// <summary>
/// Модель для обновления контакта пользователя
/// </summary>
public sealed record UpdateUserContactModel
{
    public ContactType Type { get; init; }
    public string Value { get; init; } = null!;
    public string? Description { get; init; }
}
