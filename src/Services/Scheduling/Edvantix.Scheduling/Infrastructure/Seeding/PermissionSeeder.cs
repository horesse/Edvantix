using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Infrastructure.Seeding;

/// <summary>
/// Seeds Scheduling's own permission strings into the Organizations permission catalogue on startup.
/// Uses HTTP POST to Organizations' /v1/permissions/register (not direct DB access) because
/// the Scheduling service does not own the Permission aggregate.
/// Polly retry is applied via the named HttpClient to handle startup-ordering delays when
/// Organizations is not yet fully started.
/// </summary>
internal sealed class PermissionSeeder(
    IHttpClientFactory httpClientFactory,
    ILogger<PermissionSeeder> logger
) : IHostedService
{
    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding Scheduling permission strings via Organizations HTTP...");

        // The "organizations" client has standard resilience configured in Extensions.cs,
        // which provides retry with exponential back-off for transient HTTP failures.
        var client = httpClientFactory.CreateClient("organizations");

        var response = await client.PostAsJsonAsync(
            "/v1/permissions/register",
            new { Names = SchedulingPermissions.All },
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        logger.LogInformation(
            "Scheduling permissions seeded: {Count} strings",
            SchedulingPermissions.All.Count
        );
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
