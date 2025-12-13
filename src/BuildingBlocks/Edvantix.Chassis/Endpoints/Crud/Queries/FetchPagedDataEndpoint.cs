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

public class FetchPagedDataEndpoint<TModel, TIdentity, TEntity, TSpecification>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Ok<PagedResult<TModel>>, PaginationRequest<TSpecification, TEntity>, ISender>
    where TModel : Model<TIdentity>
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
                "Возвращает пагинированные записи с учетом фильтра"
            )
            .ProducesGet<TModel>(hasNotFound: true);
    }

    public async Task<Ok<PagedResult<TModel>>> HandleAsync(
        PaginationRequest<TSpecification, TEntity> request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new FetchPagedDataQuery<TEntity, TModel, TSpecification, TIdentity>(request);
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
