using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure;

/// <summary>
/// Наполняет таблицу разрешений всеми известными кодами из
/// <see cref="OrganizationPermissions"/> и <see cref="GroupPermissions"/>.
/// Каждое разрешение проверяется индивидуально — уже существующие пропускаются.
/// </summary>
public sealed class PermissionsDbSeeder(ILogger<PermissionsDbSeeder> logger)
    : IDbSeeder<OrganizationalDbContext>
{
    private static readonly IReadOnlyList<(string Feature, string Name)> _knownPermissions =
    [
        (OrganizationPermissions.Feature, OrganizationPermissions.Create),
        (OrganizationPermissions.Feature, OrganizationPermissions.Read),
        (OrganizationPermissions.Feature, OrganizationPermissions.Update),
        (OrganizationPermissions.Feature, OrganizationPermissions.Delete),
        (OrganizationPermissions.Feature, OrganizationPermissions.TransferOwnership),
        (OrganizationPermissions.Feature, OrganizationPermissions.ManageMembers),
        (OrganizationPermissions.Feature, OrganizationPermissions.InviteMembers),
        (OrganizationPermissions.Feature, OrganizationPermissions.ManageRoles),
        (OrganizationPermissions.Feature, OrganizationPermissions.ManageGroups),
        (OrganizationPermissions.Feature, OrganizationPermissions.ViewAnalytics),
        (OrganizationPermissions.Feature, OrganizationPermissions.ManageSettings),
        (OrganizationPermissions.Feature, OrganizationPermissions.ManageSubscription),
        (GroupPermissions.Feature, GroupPermissions.Create),
        (GroupPermissions.Feature, GroupPermissions.Read),
        (GroupPermissions.Feature, GroupPermissions.Update),
        (GroupPermissions.Feature, GroupPermissions.Delete),
        (GroupPermissions.Feature, GroupPermissions.ManageMembers),
        (GroupPermissions.Feature, GroupPermissions.ViewMembers),
        (GroupPermissions.Feature, GroupPermissions.ManageContent),
        (GroupPermissions.Feature, GroupPermissions.ViewContent),
        (GroupPermissions.Feature, GroupPermissions.ManageSchedule),
    ];

    public async Task SeedAsync(OrganizationalDbContext context)
    {
        var permissions = await context.Permissions.ToListAsync();

        foreach (var (feature, name) in _knownPermissions)
        {
            var exists = permissions.Any(p => p.Feature == feature && p.Name == name);

            if (exists)
                continue;

            context.Permissions.Add(new Permission(feature, name));
            logger.LogInformation("Seeding permission {Feature}/{Name}", feature, name);
        }

        await context.SaveChangesAsync();
    }
}
