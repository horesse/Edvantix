using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Exceptions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public sealed class GetByIdQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IQueryHandler<GetByIdQuery<TModel, TIdentity>, TModel>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async ValueTask<TModel> Handle(
        GetByIdQuery<TModel, TIdentity> query,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entity = await Repository.GetByIdAsync(query.Id, token);
            if (entity == null)
                throw new NotFoundException("Entity not found");
            
            return EntityToModelMapper.Map(entity);
        }, nameof(GetByIdQuery<,>), token);
    }
}
