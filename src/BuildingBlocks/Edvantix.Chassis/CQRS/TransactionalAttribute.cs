using System.Data;

namespace Edvantix.Chassis.CQRS;

/// <summary>
/// Помечает команду как требующую обёртки в ACID-транзакцию.
/// При наличии этого атрибута конвейер <c>TransactionBehavior</c> автоматически начнёт транзакцию базы данных
/// перед выполнением обработчика и зафиксирует её при успехе (или откатит при сбое).
/// </summary>
/// <param name="isolationLevel">
/// Уровень изоляции транзакции.
/// По умолчанию <see cref="System.Data.IsolationLevel.ReadCommitted" />, который предотвращает грязное чтение
/// и подходит для большинства OLTP-команд записи.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TransactionalAttribute(
    IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
) : Attribute
{
    public IsolationLevel IsolationLevel { get; } = isolationLevel;
}
