using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class FetchPagedDataQueryHandler<TEntity, TModel, TSpecification, TIdentity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<FetchPagedDataQuery<TEntity, TModel, TSpecification>, PagedResult<TModel>>
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
    where TSpecification : class, ISpecification<TEntity>, new()
{
    public async Task<PagedResult<TModel>> Handle(
        FetchPagedDataQuery<TEntity, TModel, TSpecification> request,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var spec = request.Request.Specification ?? new TSpecification();

                spec.Take = request.Request.PageSize;
                spec.Skip = (request.Request.Page - 1) * request.Request.PageSize;

                var entities = await Repository.GetByExpressionAsync(spec, token);
                var models = entities.Select(Converter.Map).ToList();

                var totalCount = await Repository.GetCountByExpressionAsync(spec, token);

                return new PagedResult<TModel>(
                    models,
                    request.Request.Page,
                    request.Request.PageSize,
                    totalCount
                );
            },
            nameof(FetchPagedDataQuery<,,>),
            token
        );
    }
}
