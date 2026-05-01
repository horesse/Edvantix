using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Корень агрегата «Функциональная область».
/// Объединяет связанные разрешения и является единственной точкой их изменения.
/// </summary>
public sealed class Feature() : Entity, IAggregateRoot
{
    /// <param name="code">Машиночитаемый код области (например, "Organization").</param>
    /// <param name="name">Отображаемое название для UI (например, "Организация").</param>
    public Feature(string code, string name)
        : this()
    {
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Code = code.Trim();
        Name = name.Trim();
    }

    /// <summary>Машиночитаемый код области (например, "Organization").</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Отображаемое название для UI (например, "Организация").</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Разрешения, принадлежащие данной области.</summary>
    public IReadOnlyList<Permission> Permissions => _permissions;

    private readonly List<Permission> _permissions = [];

    /// <summary>Обновляет отображаемое название области.</summary>
    internal void UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    /// <summary>
    /// Добавляет разрешение к области. Если разрешение с таким кодом уже существует —
    /// обновляет его отображаемое название.
    /// </summary>
    internal void AddOrUpdatePermission(string code, string name)
    {
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        var existing = _permissions.FirstOrDefault(p =>
            p.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase)
        );

        if (existing is not null)
        {
            existing.UpdateName(name);
            return;
        }

        _permissions.Add(new Permission(this, code.Trim(), name.Trim()));
    }

    /// <summary>
    /// Синхронизирует набор разрешений: добавляет новые, удаляет отсутствующие в <paramref name="desired"/>.
    /// </summary>
    /// <returns>Количество добавленных и удалённых разрешений.</returns>
    internal (int Added, int Removed) SyncPermissions(
        IEnumerable<(string Code, string Name)> desired
    )
    {
        var desiredList = desired
            .Where(d => !string.IsNullOrWhiteSpace(d.Code))
            .Select(d => (Code: d.Code.Trim(), Name: d.Name.Trim()))
            .ToList();

        var desiredCodes = desiredList
            .Select(d => d.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toRemove = _permissions.Where(p => !desiredCodes.Contains(p.Code)).ToList();

        foreach (var p in toRemove)
            _permissions.Remove(p);

        var added = 0;
        foreach (var (code, name) in desiredList)
        {
            var existing = _permissions.FirstOrDefault(p =>
                p.Code.Equals(code, StringComparison.OrdinalIgnoreCase)
            );

            if (existing is null)
            {
                _permissions.Add(new Permission(this, code, name));
                added++;
            }
            else
            {
                existing.UpdateName(name);
            }
        }

        return (added, toRemove.Count);
    }
}
