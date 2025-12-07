using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Abstractions;

/// <summary>
/// Базовый запрос с идентификатором
/// </summary>
public abstract record BaseIdentityQuery<TIdentity, TResponse>(TIdentity Id) 
    : IQuery<TResponse> where TIdentity : struct;

/// <summary>
/// Базовая команда с идентификатором
/// </summary>
public abstract record BaseIdentityCommand<TIdentity, TResponse>(TIdentity Id) 
    : ICommand<TResponse> where TIdentity : struct;
