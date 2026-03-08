namespace Edvantix.Organizational.Features.OrganizationFeature.Features.CreateOrganization;
using Mediator;

public sealed record CreateOrganizationCommand(
    string Name,
    string NameLatin,
    string ShortName,
    OrganizationType OrganizationType,
    Guid LegalFormId,
    string? PrintName,
    string? Description
) : ICommand<Guid>;

public sealed class CreateOrganizationCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateOrganizationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();

        var organization = new Organization(
            request.Name,
            request.NameLatin,
            request.ShortName,
            DateTime.UtcNow,
            request.OrganizationType,
            request.LegalFormId,
            request.PrintName,
            request.Description
        );

        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        await orgRepo.AddAsync(organization, cancellationToken);

        // Первое сохранение — получаем organization.Id из БД
        await orgRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // Создатель становится Owner. Оба репозитория используют один DbContext (Scoped),
        // поэтому второй SaveEntities включает оба изменения в одной транзакции EF Core.
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var member = new OrganizationMember(organization.Id, profileId, OrganizationRole.Owner);
        await memberRepo.AddAsync(member, cancellationToken);
        await memberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return organization.Id;
    }
}
