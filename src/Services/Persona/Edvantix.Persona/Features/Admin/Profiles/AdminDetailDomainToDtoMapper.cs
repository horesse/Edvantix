using Edvantix.Persona.Features.Contacts;
using Edvantix.Persona.Features.Educations;
using Edvantix.Persona.Features.EmploymentHistories;
using Edvantix.Persona.Features.Skills;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Admin.Profiles;

public sealed class AdminDetailDomainToDtoMapper(
    IBlobService blobService,
    IMapper<ProfileContact, ContactDto> contactMapper,
    IMapper<EmploymentHistory, EmploymentHistoryDto> employmentHistoryMapper,
    IMapper<Education, EducationDto> educationMapper,
    IMapper<ProfileSkill, SkillDto> skillMapper
) : Mapper<Profile, AdminProfileDetailDto>
{
    public override AdminProfileDetailDto Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new AdminProfileDetailDto(
            source.Id,
            source.AccountId,
            source.Login,
            source.FullName.FirstName,
            source.FullName.LastName,
            source.FullName.MiddleName,
            source.Gender,
            source.BirthDate,
            source.Bio,
            avatarUrl,
            source.IsBlocked,
            source.LastLoginAt,
            [.. source.Contacts.Select(contactMapper.Map)],
            [.. source.EmploymentHistories.Select(employmentHistoryMapper.Map)],
            [.. source.Educations.Select(educationMapper.Map)],
            [.. source.Skills.Select(skillMapper.Map)]
        );
    }
}
