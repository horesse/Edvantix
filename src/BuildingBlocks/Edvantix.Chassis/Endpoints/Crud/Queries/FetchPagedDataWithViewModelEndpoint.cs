using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Endpoints.Requests;
using Edvantix.Chassis.Specification;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Queries;

/// <summary>
/// Generic endpoint для получения пагинированных данных с ViewViewModel
/// </summary>
public class FetchPagedDataWithViewModelEndpoint<
    TModel,
    TViewViewModel,
    TIdentity,
    TEntity,
    TSpecification
>
    : BaseCrudViewModelEndpoint<TModel, TIdentity>,
        IEndpoint<
            Ok<PagedResult<TViewViewModel>>,
            PaginationRequest<TSpecification, TEntity>,
            ISender
        >
    where TModel : Model<TIdentity>
    where TViewViewModel : class
    where TIdentity : struct
    where TEntity : class, IAggregateRoot
    where TSpecification : class, ISpecification<TEntity>, new()
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            GetRoutePath(CrudAction.FetchPagedData),
            async (
                PaginationRequest<TSpecification, TEntity> request,
                ISender sender,
                CancellationToken ct
            ) => await HandleAsync(request, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"FetchPaged{ResourceName}",
                "Получить пагинированные записи с учетом фильтра",
                "Возвращает пагинированные записи (ViewViewModel) с учетом фильтра"
            )
            .ProducesGet<TViewViewModel>(hasNotFound: true);
    }

    public async Task<Ok<PagedResult<TViewViewModel>>> HandleAsync(
        PaginationRequest<TSpecification, TEntity> request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new FetchPagedDataWithViewModelQuery<TEntity, TViewViewModel, TSpecification>(
            request
        );
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
