using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class GetByExpressionQueryHandler<TEntity, TModel, TSpecification, TIdentity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<GetByExpressionQuery<TEntity, TModel, TSpecification>, IEnumerable<TModel>>
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
    where TSpecification : ISpecification<TEntity>
{
    public async Task<IEnumerable<TModel>> Handle(
        GetByExpressionQuery<TEntity, TModel, TSpecification> request,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entities = await Repository.GetByExpressionAsync(request.Specification, token);
                return entities.Select(Converter.Map);
            },
            nameof(GetByExpressionQuery<,,>),
            token
        );
    }
}
