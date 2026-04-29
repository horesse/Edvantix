using Edvantix.Audit.Domain.Enums;
using Edvantix.Audit.Domain.Events;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;

/// <summary>
/// Запись журнала аудита — фиксирует одно действие пользователя в контексте организации.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>Запись является неизменяемой: после создания не редактируется и не удаляется.</item>
///   <item>Каждая запись привязана к организации, инициатору действия и типу сущности.</item>
/// </list>
/// </summary>
public sealed class AuditEntry() : Entity, IAggregateRoot, ITenanted
{
    /// <param name="organizationId">Идентификатор организации, в контексте которой выполнено действие.</param>
    /// <param name="actorId">Идентификатор профиля, выполнившего действие.</param>
    /// <param name="action">Тип выполненного действия.</param>
    /// <param name="entityType">Тип затронутой сущности.</param>
    /// <param name="entityId">Идентификатор затронутой сущности (если применимо).</param>
    /// <param name="description">Краткое описание действия.</param>
    /// <param name="metadata">Дополнительные данные в формате JSON.</param>
    /// <param name="ipAddress">IP-адрес инициатора действия.</param>
    /// <param name="userAgent">User-Agent клиента инициатора.</param>
    public AuditEntry(
        Guid organizationId,
        Guid actorId,
        AuditAction action,
        AuditEntityType entityType,
        Guid? entityId = null,
        string? description = null,
        string? metadata = null,
        string? ipAddress = null,
        string? userAgent = null
    )
        : this()
    {
        Id = Guid.CreateVersion7();

        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        if (actorId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор инициатора действия не может быть пустым.",
                nameof(actorId)
            );

        OrganizationId = organizationId;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        Description = description?.Trim();
        Metadata = metadata;
        IpAddress = ipAddress?.Trim();
        UserAgent = userAgent?.Trim();
        OccurredAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(
            new AuditEntryCreatedDomainEvent(Id, organizationId, actorId, action, entityType)
        );
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Идентификатор профиля, выполнившего действие.</summary>
    public Guid ActorId { get; private set; }

    /// <summary>Тип выполненного действия.</summary>
    public AuditAction Action { get; private set; }

    /// <summary>Тип затронутой сущности.</summary>
    public AuditEntityType EntityType { get; private set; }

    /// <summary>Идентификатор затронутой сущности (если применимо).</summary>
    public Guid? EntityId { get; private set; }

    /// <summary>Краткое описание действия.</summary>
    public string? Description { get; private set; }

    /// <summary>Дополнительные данные события в формате JSON.</summary>
    public string? Metadata { get; private set; }

    /// <summary>IP-адрес инициатора действия.</summary>
    public string? IpAddress { get; private set; }

    /// <summary>User-Agent клиента инициатора.</summary>
    public string? UserAgent { get; private set; }

    /// <summary>Момент времени, когда произошло действие.</summary>
    public DateTime OccurredAt { get; private set; }
}
