using System.Linq.Expressions;
using Edvantix.Chassis.Helpers;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Repository.Crud;

public abstract class CrudRepository<TContext, TEntity, TIdentity>(IServiceProvider provider)
    : ICrudRepository<TEntity, TIdentity>
    where TContext : DbContext, IUnitOfWork
    where TEntity : Entity<TIdentity>, IAggregateRoot
    where TIdentity : struct
{
    private TContext Context => provider.GetRequiredService<TContext>();
    private DbSet<TEntity> DbSet => Context.Set<TEntity>();
    
    public IUnitOfWork UnitOfWork => Context;
    
    public Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token)
        => DbSet.FirstOrDefaultAsync(predicate, token);

    public IQueryable<TEntity> GetAsQueryable()
        => DbSet.AsQueryable();

    public Task<List<TEntity>> GetAllAsync(CancellationToken token)
        => DbSet.ToListAsync(token);

    public Task<List<TEntity>> GetAllByIdsAsync(List<TIdentity> ids, CancellationToken token)
    {
        if (ids is { } identities)
        {
            return DbSet.Where(x => identities.Contains(x.Id)).ToListAsync<TEntity>(token);
        }

        return Task.FromResult(new List<TEntity>());
    }

    public Task<TEntity?> GetByIdAsync(TIdentity identity, CancellationToken token)
    {
        if (identity is TIdentity id)
        {
            return DbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), token);
        }

        return Task.FromResult(default(TEntity?));
    }

    public Task<int> GetCountAsync(CancellationToken token)
        => DbSet.CountAsync(token);

    public Task<bool> IsExistAsync(TIdentity id, CancellationToken token)
    {
        return DbSet.AnyAsync(x => x.Id.Equals(id), token);
    }

    public void Insert(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken token)
    {
        var entry = await DbSet.AddAsync(entity, token);

        return entry.Entity;
    }

    public async Task<object> InsertAsync(object entity, CancellationToken token)
    {
        var entry = await Context.AddAsync(entity, token);

        return entry.Entity;
    }

    public async Task<List<TEntity>> InsertRangeAsync(List<TEntity> entities, CancellationToken token)
    {
        var modifiedCollection = new List<TEntity>(entities.Count());

        foreach (var entity in entities)
        {
            var modifiedEntity = await InsertAsync(entity, token);

            modifiedCollection.Add(modifiedEntity);
        }

        return modifiedCollection;
    }

    public async Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken token)
    {
        EntityEntry<TEntity> entry;

        if (!entity.Id.Equals(0))
        {
            var local = Context.Set<TEntity>().Local.FindEntry(entity.Id);

            local?.State = EntityState.Detached;

            entry = Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }
        else
        {
            entry = await DbSet.AddAsync(entity, token);
        }

        return entry.Entity;
    }

    public async Task<List<TEntity>> InsertOrUpdateAsync(List<TEntity> entities, CancellationToken token)
    {
        var modifiedCollection = new List<TEntity>(entities.Count);

        foreach (var entity in entities)
        {
            var modifiedEntity = await InsertOrUpdateAsync(entity, token);

            modifiedCollection.Add(modifiedEntity);
        }

        return modifiedCollection;
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token)
    {
        var entry = Context.Entry(entity);

        if (entry == null)
        {
            throw new Exception($"Входящая сущность [{entity.Id}] отсутствует в контексте.");
        }

        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        entry.State = EntityState.Modified;
        DbSet.Update(entity);

        var result = entry.Entity;

        return Task.FromResult(result);
    }

    public async Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken token)
    {
        var modifiedCollection = new List<TEntity>(entities.Count());

        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        foreach (var entity in entities)
        {
            var modifiedEntity = await UpdateAsync(entity, token);

            modifiedCollection.Add(modifiedEntity);
        }

        return modifiedCollection;
    }

    public Task DeleteAllAsync(CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        DbSet.RemoveRange(DbSet);
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        DbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        DbSet.RemoveRange(entities);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(TIdentity id, CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        var entity = Context.Find<TEntity>(id);

        if (entity != null)
            Context.Entry(entity).State = EntityState.Deleted;
        else
            throw new Exception(
                $"Сущность c идентификатором [{id}] отсутствует в контексте."
            );

        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<TIdentity> identities, CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        foreach (var identity in identities)
        {
            DeleteAsync(identity, token);
        }

        return Task.CompletedTask;
    }

    public Task<int> ExecuteRawSqlAsync(string query, CancellationToken token)
        => Context.Database.ExecuteSqlRawAsync(query, token);

    public Task ClearCacheAsync(CancellationToken token)
    {
        if (token.GetErrorIfCancellationRequested(out var exception))
        {
            throw exception;
        }

        foreach (var entity in DbSet.Local)
        {
            DbSet.Entry(entity).State = EntityState.Detached;
        }

        return Task.CompletedTask;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token)
        => Context.Database.BeginTransactionAsync(token);
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
