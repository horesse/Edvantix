using Edvantix.Constants.Permissions;
using Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizations.Infrastructure.Seeding;

/// <summary>
/// Seeds Organizations' own permission strings into the Permission catalogue on startup.
/// Uses direct DB access (not the HTTP registration endpoint) to avoid the
/// self-calling race condition: the service cannot call itself via HTTP until it is
/// fully started, so we seed directly through the repository on IHostedService.StartAsync.
/// </summary>
internal sealed class PermissionSeeder(
    IServiceProvider serviceProvider,
    ILogger<PermissionSeeder> logger
) : IHostedService
{
    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var permissionRepository =
            scope.ServiceProvider.GetRequiredService<IPermissionRepository>();

        logger.LogInformation("Seeding Organizations permission strings...");

        await permissionRepository.UpsertAsync(OrganizationsPermissions.All, cancellationToken);
        await permissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        logger.LogInformation(
            "Organizations permissions seeded: {Count} strings",
            OrganizationsPermissions.All.Count
        );
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
