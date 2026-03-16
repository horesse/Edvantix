using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>
/// Маппер Profile → ProfileDetailsModel (полное представление с коллекциями).
/// Требует, чтобы профиль был загружен с withDetails=true
/// (Contacts, Educations, EmploymentHistories, Skills с ThenInclude Skill).
/// </summary>
public sealed class ProfileDetailsMapper(
    IBlobService blobService,
    IMapper<ProfileContact, ContactModel> contactMapper,
    IMapper<EmploymentHistory, EmploymentHistoryModel> employmentHistoryMapper,
    IMapper<Education, EducationModel> educationMapper,
    IMapper<ProfileSkill, SkillModel> skillMapper
) : Mapper<Profile, ProfileDetailsModel>
{
    public override ProfileDetailsModel Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new ProfileDetailsModel(
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
