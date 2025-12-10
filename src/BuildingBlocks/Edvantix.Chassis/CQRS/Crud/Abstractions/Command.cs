using Edvantix.Chassis.CQRS.Command;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Создание новой записи
/// </summary>
public sealed record CreateCommand<TModel, TIdentity>(TModel Model) : ICommand<TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Создание нескольких записей
/// </summary>
public sealed record CreateRangeCommand<TModel, TIdentity>(IEnumerable<TModel> Models)
    : ICommand<IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Обновление записи
/// </summary>
public sealed record UpdateCommand<TModel, TIdentity>(TModel Model) : ICommand<TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Обновление нескольких записей
/// </summary>
public sealed record UpdateRangeCommand<TModel, TIdentity>(IEnumerable<TModel> Models)
    : ICommand<IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Удаление записи
/// </summary>
public sealed record DeleteCommand<TModel, TIdentity>(TIdentity Id)
    : BaseIdentityCommand<TIdentity, TIdentity>(Id)
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Удаление нескольких записей
/// </summary>
public sealed record DeleteRangeCommand<TModel, TIdentity>(IEnumerable<TIdentity> Ids)
    : ICommand<IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Применение изменений к модели
/// </summary>
public sealed record AcceptChangesCommand<TModel, TIdentity>(TModel Model) : ICommand<bool>
    where TModel : Model<TIdentity>
    where TIdentity : struct;

/// <summary>
/// Применение изменений к нескольким моделям
/// </summary>
public sealed record AcceptChangesRangeCommand<TModel, TIdentity>(IEnumerable<TModel> Models)
    : ICommand<bool>
    where TModel : Model<TIdentity>
    where TIdentity : struct;
