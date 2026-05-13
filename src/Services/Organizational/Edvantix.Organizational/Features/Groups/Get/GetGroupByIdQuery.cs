using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.Groups.Get;

[RequirePermission(GroupPermissions.View)]
public sealed record GetGroupByIdQuery(Guid Id) : IQuery<GroupDetailDto>;

internal sealed class GetGroupByIdQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    IMapper<Group, GroupDetailDto> mapper,
    IProfileService profileService
) : IQueryHandler<GetGroupByIdQuery, GroupDetailDto>
{
    public async ValueTask<GroupDetailDto> Handle(
        GetGroupByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(query.Id, cancellationToken);
        Guard.Against.NotFound(group, query.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        var dto = mapper.Map(group);

        dto = await EnrichWithTeacherNameAsync(dto, group.TeacherMemberId, cancellationToken);
        dto = await EnrichWithRoomLabelAsync(dto, group.RoomId, cancellationToken);

        return dto;
    }

    private async Task<GroupDetailDto> EnrichWithTeacherNameAsync(
        GroupDetailDto dto,
        Guid teacherMemberId,
        CancellationToken cancellationToken
    )
    {
        var memberToProfile = await repository.GetTeacherProfileIdsAsync(
            [teacherMemberId],
            cancellationToken
        );

        if (!memberToProfile.TryGetValue(teacherMemberId, out var profileId))
            return dto;

        var response = await profileService.GetProfilesByIdsAsync(
            [profileId.ToString()],
            cancellationToken
        );

        var profile = response?.Profiles.FirstOrDefault();

        return profile is not null ? dto with { TeacherFullName = profile.FullName } : dto;
    }

    private async Task<GroupDetailDto> EnrichWithRoomLabelAsync(
        GroupDetailDto dto,
        Guid? roomId,
        CancellationToken cancellationToken
    )
    {
        if (roomId is null)
            return dto;

        var rooms = await repository.GetRoomsByIdsAsync([roomId.Value], cancellationToken);

        return rooms.TryGetValue(roomId.Value, out var room)
            ? dto with { RoomLabel = room.Label }
            : dto;
    }
}
