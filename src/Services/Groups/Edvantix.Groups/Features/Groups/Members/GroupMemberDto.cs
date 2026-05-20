using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Groups.Features.Groups.Members;

/// <summary>DTO участника учебной группы.</summary>
public sealed record GroupMemberDto(
    [property: Description("Идентификатор записи участника")] Guid Id,
    [property: Description("Идентификатор профиля пользователя")] Guid ProfileId,
    [property: Description("Полное имя участника")] string FullName,
    [property: Description("URL аватара участника")] string? AvatarUrl,
    [property: Description("Роль в группе")] GroupMemberRole Role,
    [property: Description("Дата вступления в группу")] DateOnly JoinedAt,
    [property: Description("Дата выхода из группы; null — участник активен")] DateOnly? ExitedAt,
    [property: Description("Причина выхода из группы")] string? ExitReason
);
