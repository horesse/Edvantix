using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Repository.Crud;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.CQRS.Crud.Handlers;

/// <summary>
/// Базовый обработчик с общей логикой для всех CRUD операций
/// </summary>
public abstract class BaseCrudHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
{
    protected readonly ICrudRepository<TEntity, TIdentity> Repository = provider.GetRequiredService<
        ICrudRepository<TEntity, TIdentity>
    >();
    protected readonly IConverter<TModel, TEntity> Converter = provider.GetRequiredService<
        IConverter<TModel, TEntity>
    >();

    private readonly ILogger _logger = provider.GetRequiredService<
        ILogger<BaseCrudHandler<TModel, TIdentity, TEntity>>
    >();

    protected async Task<TResponse> ExecuteAsync<TResponse>(
        Func<Task<TResponse>> operation,
        string operationName,
        CancellationToken token
    )
    {
        try
        {
            token.ThrowIfCancellationRequested();
            var result = await operation();
            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("{Operation} was cancelled", operationName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {Operation}", operationName);
            throw;
        }
    }
}
