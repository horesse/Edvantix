using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Иммутабельный журнал событий вступления и выхода участника из группы.
/// Сохраняет полную историю изменений состава с указанием причины выхода.
/// <para>
/// Таблица является журналом событий и не должна физически удаляться.
/// Поле is_deleted отсутствует намеренно.
/// </para>
/// </summary>
public sealed class GroupMembershipHistory() : Entity
{
    /// <param name="groupMemberId">Идентификатор записи участника группы.</param>
    /// <param name="joinedAt">Дата вступления участника в группу.</param>
    public GroupMembershipHistory(Guid groupMemberId, DateOnly joinedAt)
        : this()
    {
        if (groupMemberId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор участника группы не может быть пустым.",
                nameof(groupMemberId)
            );

        GroupMemberId = groupMemberId;
        JoinedAt = joinedAt;
    }

    /// <summary>Идентификатор записи участника группы.</summary>
    public Guid GroupMemberId { get; private set; }

    /// <summary>Дата вступления участника в группу.</summary>
    public DateOnly JoinedAt { get; private set; }

    /// <summary>
    /// Дата выхода участника из группы.
    /// null означает, что участник всё ещё состоит в группе.
    /// </summary>
    public DateOnly? ExitedAt { get; private set; }

    /// <summary>Причина выхода из группы (отчисление, перевод, завершение курса и т.д.).</summary>
    public string? ExitReason { get; private set; }

    /// <summary>
    /// Фиксирует выход участника из группы.
    /// </summary>
    /// <param name="exitedAt">Дата выхода.</param>
    /// <param name="exitReason">Причина выхода.</param>
    /// <exception cref="ArgumentException">Если дата выхода раньше даты вступления.</exception>
    /// <exception cref="InvalidOperationException">Если выход уже был зафиксирован.</exception>
    public void RecordExit(DateOnly exitedAt, string? exitReason = null)
    {
        if (ExitedAt.HasValue)
        {
            throw new InvalidOperationException("Выход из группы уже был зафиксирован.");
        }

        if (exitedAt < JoinedAt)
        {
            throw new ArgumentException(
                "Дата выхода не может быть раньше даты вступления.",
                nameof(exitedAt)
            );
        }

        ExitedAt = exitedAt;
        ExitReason = exitReason?.Trim();
    }
}
