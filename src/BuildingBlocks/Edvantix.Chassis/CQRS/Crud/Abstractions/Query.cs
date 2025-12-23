using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.Endpoints.Requests;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Получение всех записей
/// </summary>
public sealed record GetAllQuery<TModel, TIdentity> : IQuery<IEnumerable<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Получение записи по идентификатору
/// </summary>
public sealed record GetByIdQuery<TModel, TIdentity>(TIdentity Id)
    : BaseIdentityQuery<TIdentity, TModel>(Id)
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Получение количества записей
/// </summary>
public sealed record GetCountQuery : IQuery<long>;

/// <summary>
/// Проверка существования записи
/// </summary>
public sealed record IsExistQuery<TIdentity>(TIdentity Id) : BaseIdentityQuery<TIdentity, bool>(Id)
    where TIdentity : struct;

public sealed record GetByExpressionQuery<TEntity, TModel, TSpecification, TIdentity>(
    TSpecification Specification
) : IQuery<IEnumerable<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TSpecification : ISpecification<TEntity>
    where TEntity : class, IAggregateRoot;

public sealed record FetchPagedDataQuery<TEntity, TModel, TSpecification, TIdentity>(
    PaginationRequest<TSpecification, TEntity> Request
) : IQuery<PagedResult<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TSpecification : class, ISpecification<TEntity>
    where TEntity : class, IAggregateRoot;

/// <summary>
/// Получение пагинированных записей с маппингом в ViewViewModel
/// </summary>
public sealed record FetchPagedDataWithViewModelQuery<TEntity, TViewViewModel, TSpecification>(
    PaginationRequest<TSpecification, TEntity> Request
) : IQuery<PagedResult<TViewViewModel>>
    where TViewViewModel : class
    where TSpecification : class, ISpecification<TEntity>
    where TEntity : class, IAggregateRoot;
