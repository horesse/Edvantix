using Edvantix.Organizational.Grpc.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.CreateOrganization;

public sealed record CreateOrganizationCommand(
    string Name,
    string NameLatin,
    string ShortName,
    string? PrintName,
    string? Description
) : IRequest<Guid>;

public sealed class CreateOrganizationCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateOrganizationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var organization = new Organization(
            request.Name,
            request.NameLatin,
            request.ShortName,
            DateTime.UtcNow,
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
