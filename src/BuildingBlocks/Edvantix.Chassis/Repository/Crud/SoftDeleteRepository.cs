using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Chassis.Repository.Crud;

public abstract class SoftDeleteRepository<TContext, TEntity, TIdentity>(IServiceProvider provider)
    : CrudRepository<TContext, TEntity, TIdentity>(provider) where TContext : DbContext, IUnitOfWork
    where TEntity : Entity<TIdentity>, IAggregateRoot, ISoftDelete
    where TIdentity : struct
{
    public override Task DeleteAllAsync(CancellationToken token)
    {
        DbSet.RemoveRange(DbSet);
        return Task.CompletedTask;
    }

    public override Task DeleteAsync(TEntity entity, CancellationToken token)
    {
        entity.Delete();
        return UpdateAsync(entity, token);
    }

    public override async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken token)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, token);
        }
    }

    public override async Task DeleteAsync(TIdentity id, CancellationToken token)
    {
        var entity = await GetByIdAsync(id, token);
        
        if (entity == null)
            return;
        
        await DeleteAsync(entity, token);
    }

    public override async Task DeleteAsync(IEnumerable<TIdentity> identities, CancellationToken token)
    {
        foreach (var identity in identities)
        {
            await DeleteAsync(identity, token);
        }
    }
}
