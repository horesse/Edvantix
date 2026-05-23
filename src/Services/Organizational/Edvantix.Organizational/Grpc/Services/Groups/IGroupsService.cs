namespace Edvantix.Organizational.Grpc.Services.Groups;

/// <summary>Клиент Groups-сервиса для получения данных справочников.</summary>
public interface IGroupsService
{
    /// <summary>Возвращает количество активных и архивных уровней организации.</summary>
    Task<(int Active, int Archived)> GetLevelStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
