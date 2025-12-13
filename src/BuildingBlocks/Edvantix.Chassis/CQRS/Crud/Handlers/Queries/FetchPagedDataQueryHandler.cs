using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification.Generic;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

public class FetchPagedDataQueryHandler<TEntity, TModel, TSpecification, TIdentity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<
            FetchPagedDataQuery<TEntity, TModel, TSpecification, TIdentity>,
            PagedResult<TModel>
        >
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
    where TSpecification : PagedSpecification<TEntity>
{
    public async Task<PagedResult<TModel>> Handle(
        FetchPagedDataQuery<TEntity, TModel, TSpecification, TIdentity> request,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entities = await Repository.GetByExpressionAsync(request.Specification, token);
                var models = entities.Select(EntityToModelMapper.Map).ToList();

                request.Specification.Take = 0;
                request.Specification.Skip = 0;

                var totalCount = await Repository.GetCountByExpressionAsync(
                    request.Specification,
                    token
                );

                return new PagedResult<TModel>(
                    models,
                    request.Specification.CurrentPage,
                    request.Specification.PageSize,
                    totalCount
                );
            },
            nameof(FetchPagedDataQuery<,,,>),
            token
        );
    }
}
