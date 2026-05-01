using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Infrastructure;

/// <summary>
/// Наполняет таблицы функциональных областей и разрешений всеми известными значениями
/// из <see cref="OrganizationPermission"/> и <see cref="GroupPermission"/>.
/// Для существующих записей обновляются отображаемые названия, если они изменились.
/// </summary>
public sealed class PermissionsDbSeeder(ILogger<PermissionsDbSeeder> logger)
    : IDbSeeder<OrganizationalDbContext>
{
    private static IEnumerable<(
        string FeatureCode,
        string FeatureName,
        string Code,
        string Name
    )> GetKnownPermissions()
    {
        foreach (var value in Enum.GetValues<OrganizationPermission>())
            yield return (
                nameof(OrganizationPermission),
                typeof(OrganizationPermission).GetDisplayName(),
                value.GetCode(),
                value.GetDisplayName()
            );
    }

    public async Task SeedAsync(OrganizationalDbContext context)
    {
        // AutoInclude обеспечивает загрузку Permissions вместе с Feature.
        var features = await context.Features.ToListAsync();

        foreach (var group in GetKnownPermissions().GroupBy(p => p.FeatureCode))
        {
            var featureCode = group.Key;
            var featureName = group.First().FeatureName;

            var feature = features.FirstOrDefault(f => f.Code == featureCode);

            if (feature is null)
            {
                feature = new Feature(featureCode, featureName);
                context.Features.Add(feature);
                features.Add(feature);
                logger.LogInformation("Добавлена область {Code}", featureCode);
            }
            else if (feature.Name != featureName)
            {
                feature.UpdateName(featureName);
                logger.LogInformation("Обновлено название области {Code}", featureCode);
            }

            foreach (var (_, _, code, name) in group)
            {
                var before = feature.Permissions.Count;
                feature.AddOrUpdatePermission(code, name);
                if (feature.Permissions.Count > before)
                    logger.LogInformation(
                        "Добавлено разрешение {FeatureCode}/{Code}",
                        featureCode,
                        code
                    );
            }
        }

        await context.SaveChangesAsync();
    }
}
