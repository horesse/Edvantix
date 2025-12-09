using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public sealed class GetAllByIdsQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<GetAllByIdsQuery<TModel, TIdentity>, IEnumerable<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<IEnumerable<TModel>> Handle(
        GetAllByIdsQuery<TModel, TIdentity> query,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entities = await Repository.GetAllByIdsAsync(query.Ids, token);
            return entities.Select(EntityToModelMapper.Map);
        }, nameof(GetAllByIdsQuery<TModel, TIdentity>), token);
    }
}
