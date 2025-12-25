using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Crud.Abstractions;

namespace Edvantix.Person.CQRS;

/// <summary>
/// Создание новой записи PersonalData
/// </summary>
public sealed record PersonalDataCreateCommand<TModel, TIdentity>(TModel Model)
    : ICommand<TIdentity>,
        IPersonalDataCommand
    where TModel : class
    where TIdentity : struct
{
    public long PersonInfoId { get; set; }
}

/// <summary>
/// Обновление записи PersonalData
/// </summary>
public sealed record PersonalDataUpdateCommand<TModel, TIdentity>(TIdentity Id, TModel Model)
    : ICommand<TIdentity>,
        IPersonalDataCommand
    where TModel : class
    where TIdentity : struct
{
    public long PersonInfoId { get; set; }
}

/// <summary>
/// Удаление записи PersonalData с проверкой владельца
/// </summary>
public sealed record PersonalDataDeleteCommand<TIdentity>(TIdentity Id)
    : BaseIdentityCommand<TIdentity, TIdentity>(Id),
        IPersonalDataCommand
    where TIdentity : struct
{
    public long PersonInfoId { get; set; }
}
