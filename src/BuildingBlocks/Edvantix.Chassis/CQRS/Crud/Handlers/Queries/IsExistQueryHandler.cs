using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public sealed class IsExistQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IQueryHandler<IsExistQuery<TIdentity>, bool>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async ValueTask<bool> Handle(
        IsExistQuery<TIdentity> query,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            return await Repository.IsExistAsync(query.Id, token);
        }, nameof(IsExistQuery<TIdentity>), token);
    }
}
