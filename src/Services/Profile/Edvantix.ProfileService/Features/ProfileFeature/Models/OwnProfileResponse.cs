namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

public sealed record OwnProfileResponse(string Id, string Name, string UserName, string? AvatarUrl);
