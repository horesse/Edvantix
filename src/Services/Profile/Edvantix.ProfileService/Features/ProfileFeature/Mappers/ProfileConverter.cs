using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.EducationFeature.Models;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using Edvantix.ProfileService.Infrastructure.Blob;

namespace Edvantix.ProfileService.Features.ProfileFeature.Mappers;

public sealed class ProfileConverter(IServiceProvider provider)
    : ClassConverter<ProfileModel, Profile>
{
    public override Profile Map(ProfileModel source) => throw new NotImplementedException();

    public override ProfileModel Map(Profile source)
    {
        var blobService = provider.GetRequiredService<IBlobService>();
        var avatarUrl = source.Avatar is not null ? blobService.GetFileSasUrl(source.Avatar) : null;

        return new ProfileModel
        {
            AccountId = source.AccountId,
            AvatarUrl = avatarUrl,
            FirstName = source.FullName.FirstName,
            LastName = source.FullName.LastName,
            MiddleName = source.FullName.MiddleName,
            Gender = source.Gender,
            Id = source.Id,
            BirthDate = source.BirthDate,
            Contacts = source.Contacts.Select(provider.Map<UserContactModel, UserContact>),
            EmploymentHistories = source.EmploymentHistories.Select(
                provider.Map<EmploymentHistoryModel, EmploymentHistory>
            ),
            Educations = source.Educations.Select(provider.Map<EducationModel, Education>),
        };
    }

    public override void SetProperties(ProfileModel source, Profile target)
    {
        target.UpdateGender(source.Gender);

        target.UpdateBirthDate(source.BirthDate);
        target.UpdateFullName(source.FirstName, source.LastName, source.MiddleName);

        target.UploadAvatar(source.AvatarUrl);

        if (source.Contacts is not null)
        {
            var converter = provider.GetRequiredService<IConverter<UserContactModel, UserContact>>();
            var contacts = converter.Map(source.Contacts.ToList());
            target.ReplaceContacts(contacts);
        }

        if (source.EmploymentHistories is not null)
        {
            var converter = provider.GetRequiredService<IConverter<EmploymentHistoryModel, EmploymentHistory>>();
            var employmentHistories = converter.Map(source.EmploymentHistories.ToList());
            target.ReplaceEmploymentHistories(employmentHistories);
        }

        if (source.Educations is not null)
        {
            var converter = provider.GetRequiredService<IConverter<EducationModel, Education>>();
            var educations = converter.Map(source.Educations.ToList());
            target.ReplaceEducations(educations);
        }
    }
}
