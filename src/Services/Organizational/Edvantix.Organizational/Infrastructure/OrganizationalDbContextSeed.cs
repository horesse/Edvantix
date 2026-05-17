using Edvantix.Permissions;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure;

/// <summary>
/// Синхронизирует таблицы <c>features</c> и <c>permissions</c> со всеми зарегистрированными
/// <see cref="PermissionModule"/>. Добавляет новые, обновляет изменившиеся названия,
/// удаляет устаревшие записи из модулей данного сервиса.
/// </summary>
public sealed class PermissionsDbSeeder(
    IEnumerable<PermissionModule> modules,
    ILogger<PermissionsDbSeeder> logger
) : IDbSeeder<OrganizationalDbContext>
{
    public async Task SeedAsync(OrganizationalDbContext context)
    {
        var moduleList = modules.ToList();

        await SeedFeaturesAsync(context, moduleList);
        await SeedPermissionsAsync(context, moduleList);

        await context.SaveChangesAsync();
    }

    private async Task SeedFeaturesAsync(
        OrganizationalDbContext context,
        List<PermissionModule> moduleList
    )
    {
        var existing = await context.Features.ToListAsync();
        var byCode = existing.ToDictionary(f => f.Code, StringComparer.OrdinalIgnoreCase);

        var featuresUpdated = 0;
        var featuresAdded = 0;

        foreach (var module in moduleList)
        {
            if (byCode.TryGetValue(module.FeatureCode, out var feature))
            {
                if (feature.Name != module.FeatureName)
                {
                    feature.Update(module.FeatureName);
                    featuresUpdated++;
                }
            }
            else
            {
                context.Features.Add(
                    new Feature(module.ServiceCode, module.FeatureCode, module.FeatureName)
                );
                featuresAdded++;
                logger.LogInformation("Добавлена область {FeatureCode}", module.FeatureCode);
            }
        }

        // Удаляем только области текущего сервиса, которых нет в модулях.
        var serviceCodes = moduleList
            .Select(m => m.ServiceCode)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var desiredCodes = moduleList
            .Select(m => m.FeatureCode)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toRemove = existing
            .Where(f => serviceCodes.Contains(f.ServiceCode) && !desiredCodes.Contains(f.Code))
            .ToList();

        if (toRemove.Count > 0)
        {
            context.Features.RemoveRange(toRemove);
            logger.LogInformation("Удалено устаревших областей: {Count}", toRemove.Count);
        }

        logger.LogInformation(
            "Области: добавлено {Added}, обновлено {Updated}",
            featuresAdded,
            featuresUpdated
        );
    }

    private async Task SeedPermissionsAsync(
        OrganizationalDbContext context,
        List<PermissionModule> moduleList
    )
    {
        var existing = await context.Permissions.ToListAsync();
        var byFullCode = existing.ToDictionary(
            p => $"{p.FeatureCode}.{p.Code}",
            StringComparer.OrdinalIgnoreCase
        );

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
                if (perm.Name != entry.Name)
                {
                    perm.Update(entry.Name);
                    updated++;
                }
            }
            else
            {
                context.Permissions.Add(new Permission(module.FeatureCode, entry.Code, entry.Name));
                added++;
                logger.LogInformation("Добавлено разрешение {FullCode}", fullCode);
            }
        }

        // Удаляем только разрешения текущего сервиса, которых нет в модулях.
        var serviceCodes = moduleList
            .Select(m => m.ServiceCode)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingFeatureCodes = await context
            .Features.Where(f => serviceCodes.Contains(f.ServiceCode))
            .Select(f => f.Code)
            .ToListAsync();

        var toRemove = existing
            .Where(p =>
                existingFeatureCodes.Contains(p.FeatureCode)
                && !desiredFullCodes.Contains($"{p.FeatureCode}.{p.Code}")
            )
            .ToList();

        if (toRemove.Count > 0)
        {
            context.Permissions.RemoveRange(toRemove);
            logger.LogInformation("Удалено устаревших разрешений: {Count}", toRemove.Count);
        }

        logger.LogInformation("Разрешения: добавлено {Added}, обновлено {Updated}", added, updated);
    }
}
