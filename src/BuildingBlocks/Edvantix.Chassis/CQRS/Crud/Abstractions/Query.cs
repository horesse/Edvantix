using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.Endpoints.Requests;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Получение всех записей
/// </summary>
public sealed record GetAllQuery<TModel, TIdentity> : IQuery<IEnumerable<TModel>>
    where TModel : class
    where TIdentity : struct;

/// <summary>
/// Получение записи по идентификатору
/// </summary>
public sealed record GetByIdQuery<TModel, TIdentity>(TIdentity Id)
    : BaseIdentityQuery<TIdentity, TModel>(Id)
    where TModel : class
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

public sealed record GetByExpressionQuery<TEntity, TModel, TSpecification>(
    TSpecification Specification
) : IQuery<IEnumerable<TModel>>
    where TEntity : class
    where TModel : class
    where TSpecification : ISpecification<TEntity>;

public sealed record FetchPagedDataQuery<TEntity, TModel, TSpecification>(
    PaginationRequest<TSpecification, TEntity> Request
) : IQuery<PagedResult<TModel>>
    where TEntity : class
    where TModel : class
    where TSpecification : class, ISpecification<TEntity>;
