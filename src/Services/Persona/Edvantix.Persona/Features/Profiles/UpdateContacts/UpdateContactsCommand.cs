using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateContacts;

/// <summary>PATCH /v1/profile/contacts — обновить контакты профиля.</summary>
public sealed record UpdateContactsCommand(
    List<ContactRequest> Contacts
) : IRequest<ProfileDetailsModel>;

public sealed class UpdateContactsCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateContactsCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateContactsCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.ReplaceContacts(
            command.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
