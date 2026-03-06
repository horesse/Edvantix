using System.Text.Json;
using StackExchange.Redis;

namespace Edvantix.Catalog.Infrastructure.Idempotency;

/// <summary>
/// Redis-реализация менеджера идемпотентности.
/// Использует SemaphoreSlim для безопасного получения database-соединения
/// и System.Text.Json с AOT source generation для сериализации.
/// </summary>
internal sealed class RequestManager(IConnectionMultiplexer redis) : IRequestManager
{
    /// <summary>Время хранения записи в Redis (1 час).</summary>
    private const int DefaultExpirationTime = 3600;

    // SemaphoreSlim защищает получение IDatabase при конкурентных вызовах
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    /// <inheritdoc />
    public async Task<bool> IsExistAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default
    )
    {
        var database = await GetDatabaseAsync();
        return await database.KeyExistsAsync(idempotencyKey);
    }

    /// <inheritdoc />
    public async Task CreateAsync(
        ClientRequest clientRequest,
        CancellationToken cancellationToken = default
    )
    {
        var database = await GetDatabaseAsync();

        var json = JsonSerializer.Serialize(
            clientRequest,
            IdempotencySerializationContext.Default.ClientRequest
        );

        await database.StringSetAsync(
            clientRequest.Id,
            json,
            TimeSpan.FromSeconds(DefaultExpirationTime)
        );
    }

    private async Task<IDatabase> GetDatabaseAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            return redis.GetDatabase();
        }
        finally
        {
            _connectionLock.Release();
        }
    }
}

/// <summary>
/// Source-generation контекст для AOT-сериализации ClientRequest.
/// Исключает reflection-based сериализацию в hot path.
/// </summary>
[JsonSerializable(typeof(ClientRequest))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal sealed partial class IdempotencySerializationContext : JsonSerializerContext;
