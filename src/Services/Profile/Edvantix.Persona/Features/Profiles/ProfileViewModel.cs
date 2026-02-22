namespace Edvantix.Persona.Features.Profiles;

/// <summary>Краткое представление профиля: отображается в списках и заголовках.</summary>
public sealed record ProfileViewModel(Guid Id, string Name, string UserName, string? AvatarUrl);
