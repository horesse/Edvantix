namespace Edvantix.Permissions;

/// <summary>
/// Описание одного разрешения: машиночитаемый код и отображаемое название.
/// Используется <see cref="PermissionModule"/> для декларативного объявления набора разрешений.
/// </summary>
public sealed record PermissionEntry(string Code, string Name);
