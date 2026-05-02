using Edvantix.Chassis.Permissions;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure;

/// <summary>
/// Синхронизирует таблицу разрешений со всеми зарегистрированными <see cref="PermissionModule"/>.
/// Добавляет новые, обновляет изменившиеся названия, удаляет устаревшие разрешения
/// из модулей, принадлежащих данному сервису.
/// </summary>
public sealed class PermissionsDbSeeder(
    IEnumerable<PermissionModule> modules,
    ILogger<PermissionsDbSeeder> logger
) : IDbSeeder<OrganizationalDbContext>
{
    public async Task SeedAsync(OrganizationalDbContext context)
    {
        var existing = await context.Permissions.ToListAsync();
        var byFullCode = existing.ToDictionary(
            p => $"{p.FeatureCode}.{p.Code}",
            StringComparer.OrdinalIgnoreCase
        );

        var moduleList = modules.ToList();
        var desired = moduleList
            .SelectMany(m => m.GetPermissions().Select(e => (Module: m, Entry: e)))
            .ToList();

        var desiredFullCodes = desired
            .Select(x => $"{x.Module.FeatureCode}.{x.Entry.Code}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var added = 0;
        var updated = 0;

        foreach (var (module, entry) in desired)
        {
            var fullCode = $"{module.FeatureCode}.{entry.Code}";

            if (byFullCode.TryGetValue(fullCode, out var perm))
            {
                if (perm.FeatureName != module.FeatureName || perm.Name != entry.Name)
                {
                    perm.Update(module.FeatureName, entry.Name);
                    updated++;
                }
            }
            else
            {
                context.Permissions.Add(
                    new Permission(
                        module.ServiceCode,
                        module.FeatureCode,
                        module.FeatureName,
                        entry.Code,
                        entry.Name
                    )
                );
                added++;
                logger.LogInformation("Добавлено разрешение {FullCode}", fullCode);
            }
        }

        // Удаляем только разрешения текущего сервиса, которых нет в модулях.
        var serviceCodes = moduleList
            .Select(m => m.ServiceCode)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toRemove = existing
            .Where(p =>
                serviceCodes.Contains(p.ServiceCode)
                && !desiredFullCodes.Contains($"{p.FeatureCode}.{p.Code}")
            )
            .ToList();

        if (toRemove.Count > 0)
        {
            context.Permissions.RemoveRange(toRemove);
            logger.LogInformation("Удалено устаревших разрешений: {Count}", toRemove.Count);
        }

        await context.SaveChangesAsync();
        logger.LogInformation(
            "Сидирование разрешений завершено: добавлено {Added}, обновлено {Updated}",
            added,
            updated
        );
    }
}
