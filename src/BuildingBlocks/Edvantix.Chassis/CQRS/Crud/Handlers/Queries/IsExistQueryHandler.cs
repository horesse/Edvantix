using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public sealed class IsExistQueryHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<IsExistQuery<TIdentity>, bool>
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
{
    public async Task<bool> Handle(IsExistQuery<TIdentity> query, CancellationToken token)
    {
        return await ExecuteAsync(
            async () =>
            {
                return await Repository.IsExistAsync(query.Id, token);
            },
            nameof(IsExistQuery<TIdentity>),
            token
        );
    }
}
