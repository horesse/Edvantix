using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Groups.Features.Groups.Members;

/// <summary>
/// Маппер GroupMember → GroupMemberDto (без обогащения профилем из Persona).
/// Поля <see cref="GroupMemberDto.FullName"/> и <see cref="GroupMemberDto.AvatarUrl"/>
/// заполняются пустыми значениями — обогащение производится в обработчике запроса.
/// </summary>
public sealed class GroupMemberDomainToDtoMapper : Mapper<GroupMember, GroupMemberDto>
{
    public override GroupMemberDto Map(GroupMember source) =>
        new(
            source.Id,
            source.ProfileId,
            FullName: string.Empty,
            AvatarUrl: null,
            source.Role,
            source.JoinedAt,
            source.ExitedAt,
            source.ExitReason
        );
}
