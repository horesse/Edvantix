using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Получение всех записей
/// </summary>
public sealed record GetAllQuery<TModel, TIdentity> : IQuery<IEnumerable<TModel>>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Получение записей по списку идентификаторов
/// </summary>
public sealed record GetAllByIdsQuery<TModel, TIdentity>(List<TIdentity> Ids)
    : IQuery<IEnumerable<TModel>>
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
public sealed record IsExistQuery<TIdentity>(TIdentity Id)
    : BaseIdentityQuery<TIdentity, bool>(Id) where TIdentity : struct;
