using Edvantix.Chassis.CQRS.Command;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Создание новой записи
/// </summary>
public sealed record CreateCommand<TModel, TIdentity>(TModel Model) : ICommand<TIdentity>
    where TModel : class
    where TIdentity : struct;

/// <summary>
/// Обновление записи
/// </summary>
public sealed record UpdateCommand<TModel, TIdentity>(TIdentity Id, TModel Model)
    : ICommand<TIdentity>
    where TModel : class
    where TIdentity : struct;

/// <summary>
/// Удаление записи
/// </summary>
public sealed record DeleteCommand<TIdentity>(TIdentity Id)
    : BaseIdentityCommand<TIdentity, TIdentity>(Id)
    where TIdentity : struct;

/// <summary>
/// Удаление нескольких записей
/// </summary>
public sealed record DeleteRangeCommand<TIdentity>(IEnumerable<TIdentity> Ids)
    : ICommand<IEnumerable<TIdentity>>
    where TIdentity : struct;
