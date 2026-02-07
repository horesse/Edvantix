using System.ComponentModel;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.Endpoints.Requests;
using Edvantix.Chassis.Specification;
using Edvantix.Constants.Core;
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
    [property: Description("Спецификация")] TSpecification Specification,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description(
        "Количество элементов, которые должны быть отображены на одной странице результатов."
    )]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Свойство для упорядочивания результатов")] string? OrderBy = null,
    [property: Description("При выборе порядка сортировки результат будет в порядке убывания.")]
    [property: DefaultValue(false)]
        bool IsDescending = false
) : IQuery<PagedResult<TModel>>
    where TEntity : class
    where TModel : class
    where TSpecification : class, ISpecification<TEntity>;
