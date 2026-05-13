using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.OrganizationMembers.Teachers;

/// <summary>Возвращает список активных участников организации для выбора преподавателя.</summary>
/// <remarks>
/// Профильные данные (ФИО, аватар) запрашиваются из сервиса Persona через gRPC.
/// Фильтрация по <paramref name="Search"/> выполняется по <c>FullName</c> после обогащения данными профиля.
/// </remarks>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetTeachersQuery(
    [property: Description("Поисковая строка по имени преподавателя")]
    string? Search = null
) : IQuery<IReadOnlyCollection<TeacherDto>>;

internal sealed class GetTeachersQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository,
    IProfileService profileService
) : IQueryHandler<GetTeachersQuery, IReadOnlyCollection<TeacherDto>>
{
    public async ValueTask<IReadOnlyCollection<TeacherDto>> Handle(
        GetTeachersQuery request,
        CancellationToken cancellationToken
    )
    {
        var organizationId = tenantContext.OrganizationId;

        // Загружаем всех активных участников; Role подтягивается через AutoInclude.
        var spec = new OrganizationMemberSpecification(organizationId, status: OrganizationStatus.Active);
        var members = await repository.ListAsync(spec, cancellationToken);

        if (members.Count == 0)
            return [];

        var profileIds = members.Select(m => m.ProfileId.ToString()).ToArray();
        var response = await profileService.GetProfilesByIdsAsync(profileIds, cancellationToken);
        Guard.Against.Null(response, nameof(response));

        var profiles = response.Profiles.ToDictionary(p => p.Id);

        var teachers = members
            .Select(m =>
            {
                var profileId = m.ProfileId.ToString();
                var profile = profiles.GetValueOrDefault(profileId);

                return new TeacherDto(
                    MemberId: m.Id,
                    FullName: profile?.FullName ?? string.Empty,
                    PrimaryRole: m.Role?.Name ?? string.Empty,
                    AvatarUrl: profile is { HasAvatarUrl: true } ? profile.AvatarUrl : null
                );
            })
            .Where(t =>
                string.IsNullOrWhiteSpace(request.Search)
                || t.FullName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
            )
            .OrderBy(t => t.FullName)
            .ToList();

        return teachers;
    }
}
