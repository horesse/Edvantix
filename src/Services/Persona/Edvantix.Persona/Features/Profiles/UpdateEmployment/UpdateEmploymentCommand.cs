using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateEmployment;

/// <summary>PATCH /v1/profile/employment — обновить опыт работы профиля.</summary>
public sealed record UpdateEmploymentCommand(
    List<EmploymentHistoryRequest> EmploymentHistories
) : IRequest<ProfileDetailsModel>;

public sealed class UpdateEmploymentCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateEmploymentCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateEmploymentCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.ReplaceEmploymentHistories(
            command.EmploymentHistories.Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
