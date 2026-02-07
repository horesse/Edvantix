using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class GetByExpressionQueryHandler<TEntity, TModel, TSpecification, TIdentity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<GetByExpressionQuery<TEntity, TModel, TSpecification>, PagedResult<TModel>>
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
    where TSpecification : class, ISpecification<TEntity>
{
    public async Task<PagedResult<TModel>> Handle(
        GetByExpressionQuery<TEntity, TModel, TSpecification> request,
        CancellationToken token
    )
    {
        var count = await Repository.GetCountByExpressionAsync(request.Specification, token);

        request.Specification.Skip = (request.PageIndex - 1) * request.PageSize;
        request.Specification.Take = request.PageSize;

        var data = await ExecuteAsync(
            async () =>
            {
                var entities = await Repository.GetByExpressionAsync(request.Specification, token);
                return entities.Select(Converter.Map);
            },
            nameof(GetByExpressionQuery<,,>),
            token
        );

        return new PagedResult<TModel>([.. data], request.PageIndex, request.PageSize, count);
    }
}
