namespace Edvantix.Organizational.Grpc.Services.Groups;

/// <summary>
/// Сервис для получения статистики использования записей справочников в учебных группах.
/// Данные запрашиваются у сервиса Groups по gRPC (batch-запрос).
/// </summary>
internal interface IGroupsUsageService
{
    /// <summary>Возвращает число активных групп для каждого уровня из списка.</summary>
    /// <param name="levelIds">Идентификаторы уровней.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Словарь: levelId → количество групп (0 для уровней без групп не включается).</returns>
    Task<IReadOnlyDictionary<Guid, int>> CountByLevelIdsAsync(
        IEnumerable<Guid> levelIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает число активных групп для каждого кабинета из списка.</summary>
    /// <param name="roomIds">Идентификаторы кабинетов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Словарь: roomId → количество групп (0 для кабинетов без групп не включается).</returns>
    Task<IReadOnlyDictionary<Guid, int>> CountByRoomIdsAsync(
        IEnumerable<Guid> roomIds,
        CancellationToken cancellationToken = default
    );
}
