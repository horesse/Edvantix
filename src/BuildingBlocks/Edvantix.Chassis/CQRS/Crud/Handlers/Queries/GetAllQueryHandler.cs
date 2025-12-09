using System.Diagnostics.CodeAnalysis;
using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class GetAllQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IQueryHandler<GetAllQuery<TModel, TIdentity>, IEnumerable<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async ValueTask<IEnumerable<TModel>> Handle(
        GetAllQuery<TModel, TIdentity> query,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entities = await Repository.GetAllAsync(token);
            return entities.Select(EntityToModelMapper.Map);
        }, nameof(GetAllQuery<,>), token);
    }
}
