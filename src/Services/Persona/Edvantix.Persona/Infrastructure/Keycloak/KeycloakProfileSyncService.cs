namespace Edvantix.Persona.Infrastructure.Keycloak;

// Запускается только в Development окружении.
// Восстанавливает атрибут profileId в Keycloak для всех профилей из БД,
// так как Keycloak не использует persistent volume и теряет атрибуты при перезапуске.
internal sealed class KeycloakProfileSyncService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    ILogger<KeycloakProfileSyncService> logger
) : BackgroundService
{
    private const int MaxDbAttempts = 10;
    private static readonly TimeSpan DbRetryDelay = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!environment.IsDevelopment())
            return;

        logger.LogInformation("Запуск синхронизации profileId в Keycloak (Development)...");

        await WaitForDatabaseAsync(stoppingToken);
        await SyncProfilesAsync(stoppingToken);
    }

    private async Task WaitForDatabaseAsync(CancellationToken ct)
    {
        for (var attempt = 1; attempt <= MaxDbAttempts; attempt++)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<PersonaDbContext>();

                await db.Database.CanConnectAsync(ct);
                return;
            }
            catch (Exception ex) when (attempt < MaxDbAttempts)
            {
                logger.LogWarning(
                    "База данных недоступна (попытка {Attempt}/{MaxAttempts}): {Message}",
                    attempt,
                    MaxDbAttempts,
                    ex.Message
                );

                await Task.Delay(DbRetryDelay, ct);
            }
        }
    }

    private async Task SyncProfilesAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PersonaDbContext>();
        var keycloak = scope.ServiceProvider.GetRequiredService<IKeycloakAdminService>();

        // Читаем только нужные поля — нет смысла загружать весь граф агрегата
        var profiles = await db.Set<Profile>()
            .AsNoTracking()
            .Select(p => new { p.Id, p.AccountId })
            .ToListAsync(ct);

        logger.LogInformation("Найдено {Count} профилей для синхронизации.", profiles.Count);

        var synced = 0;
        var failed = 0;

        foreach (var profile in profiles)
        {
            try
            {
                await keycloak.SetProfileIdAsync(profile.AccountId, profile.Id, ct);
                synced++;
            }
            catch (Exception ex)
            {
                // Ошибка одного пользователя не останавливает синхронизацию остальных
                logger.LogError(
                    ex,
                    "Ошибка синхронизации profileId для аккаунта {AccountId}.",
                    profile.AccountId
                );
                failed++;
            }
        }

        logger.LogInformation(
            "Синхронизация Keycloak завершена: успешно {Synced}, с ошибками {Failed}.",
            synced,
            failed
        );
    }
}
