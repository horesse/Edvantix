using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Queries;

/// <summary>
/// Обработчик получения пагинированных данных с маппингом в ViewViewModel
/// </summary>
public class FetchPagedDataWithViewModelQueryHandler<
    TEntity,
    TModel,
    TViewViewModel,
    TSpecification,
    TIdentity
>(IServiceProvider provider)
    : BaseCrudViewModelHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<
            FetchPagedDataWithViewModelQuery<TEntity, TViewViewModel, TSpecification>,
            PagedResult<TViewViewModel>
        >
    where TModel : Model<TIdentity>
    where TViewViewModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
    where TSpecification : class, ISpecification<TEntity>, new()
{
    private readonly Func<TModel, TViewViewModel> _modelToViewModelMapper =
        provider.GetRequiredService<Func<TModel, TViewViewModel>>();

    public async Task<PagedResult<TViewViewModel>> Handle(
        FetchPagedDataWithViewModelQuery<TEntity, TViewViewModel, TSpecification> request,
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

                // Маппинг Entity -> Model -> ViewViewModel
                var viewModels = entities
                    .Select(entity => Converter.Map(entity))
                    .Select(model => _modelToViewModelMapper(model))
                    .ToList();

                var totalCount = await Repository.GetCountByExpressionAsync(spec, token);

                return new PagedResult<TViewViewModel>(
                    viewModels,
                    request.Request.Page,
                    request.Request.PageSize,
                    totalCount
                );
            },
            nameof(FetchPagedDataWithViewModelQuery<,,>),
            token
        );
    }
}
