using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class GetByExpressionQueryHandler<TEntity, TModel, TSpecification, TIdentity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<
            GetByExpressionQuery<TEntity, TModel, TSpecification, TIdentity>,
            IEnumerable<TModel>
        >
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
    where TSpecification : ISpecification<TEntity>
{
    public async Task<IEnumerable<TModel>> Handle(
        GetByExpressionQuery<TEntity, TModel, TSpecification, TIdentity> request,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entities = await Repository.GetByExpressionAsync(request.Specification, token);
                return entities.Select(Converter.Map);
            },
            nameof(GetByExpressionQuery<,,,>),
            token
        );
    }
}
