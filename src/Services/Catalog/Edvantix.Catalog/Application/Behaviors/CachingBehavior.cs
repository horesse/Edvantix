namespace Edvantix.Catalog.Application.Behaviors;

/// <summary>
/// Pipeline-behavior для прозрачного кэширования ответов запросов, реализующих <see cref="ICachedQuery"/>.
/// При cache-hit пропускает вызов handler'а и возвращает результат из <see cref="HybridCache"/>.
/// При cache-miss выполняет handler и сохраняет результат в кэш с тегами для последующей инвалидации.
/// </summary>
/// <typeparam name="TMessage">Тип сообщения (должен реализовывать <see cref="ICachedQuery"/>).</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public sealed class CachingBehavior<TMessage, TResponse>(HybridCache cache)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, ICachedQuery
{
    /// <inheritdoc/>
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // Передаём message и next как state, чтобы избежать closure-аллокаций
        var state = (message, next);

        return await cache.GetOrCreateAsync(
            message.CacheKey,
            state,
            static async (s, ct) => await s.next(s.message, ct),
            new HybridCacheEntryOptions { Expiration = message.Expiry },
            tags: message.Tags,
            cancellationToken: cancellationToken
        );
    }
}
