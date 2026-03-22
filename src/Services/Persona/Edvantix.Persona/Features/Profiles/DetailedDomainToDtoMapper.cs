using Edvantix.Persona.Features.Contacts;
using Edvantix.Persona.Features.Educations;
using Edvantix.Persona.Features.EmploymentHistories;
using Edvantix.Persona.Features.Skills;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles;

public sealed class DetailedDomainToDtoMapper(
    IBlobService blobService,
    IMapper<ProfileContact, ContactDto> contactMapper,
    IMapper<EmploymentHistory, EmploymentHistoryDto> employmentHistoryMapper,
    IMapper<Education, EducationDto> educationMapper,
    IMapper<ProfileSkill, SkillDto> skillMapper
) : Mapper<Profile, ProfileDetailsDto>
{
    public override ProfileDetailsDto Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new ProfileDetailsDto(
            source.Id,
            source.AccountId,
            source.Login,
            source.Gender,
            source.BirthDate,
            source.FullName.FirstName,
            source.FullName.LastName,
            source.FullName.MiddleName,
            avatarUrl,
            source.Bio,
            [.. source.Contacts.Select(contactMapper.Map)],
            [.. source.EmploymentHistories.Select(employmentHistoryMapper.Map)],
            [.. source.Educations.Select(educationMapper.Map)],
            [.. source.Skills.Select(skillMapper.Map)]
        );
    }
}
