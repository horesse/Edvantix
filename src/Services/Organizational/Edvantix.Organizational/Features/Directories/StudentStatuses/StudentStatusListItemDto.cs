using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>Краткое DTO статуса студента для отображения в списке.</summary>
/// <param name="Id">Идентификатор записи (ключ строки в UI).</param>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Машинный код.</param>
/// <param name="Tone">Визуальный тон UI.</param>
/// <param name="IsSystem">Системная запись.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
public sealed record StudentStatusListItemDto(
    Guid Id,
    string Name,
    string Code,
    StudentStatusTone Tone,
    bool IsSystem,
    bool IsArchived,
    int Order
);
