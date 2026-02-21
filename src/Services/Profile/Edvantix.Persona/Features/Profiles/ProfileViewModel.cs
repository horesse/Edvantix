namespace Edvantix.Persona.Features.Profiles;

/// <summary>Краткое представление профиля: отображается в списках и заголовках.</summary>
public sealed record ProfileViewModel(ulong Id, string Name, string UserName, string? AvatarUrl);
