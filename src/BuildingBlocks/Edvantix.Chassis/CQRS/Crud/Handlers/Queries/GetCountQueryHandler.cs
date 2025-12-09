using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public sealed class GetCountQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<GetCountQuery, long>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<long> Handle(
        GetCountQuery query,
        CancellationToken token)
    {
        return await ExecuteAsync(async () => await Repository.GetCountAsync(token),
            nameof(GetCountQuery), token);
    }
}
