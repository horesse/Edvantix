using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.AcceptInvitation;

/// <summary>
/// Команда принятия приглашения по токену.
/// </summary>
public sealed record AcceptInvitationCommand(Guid Token) : ICommand<Guid>;

/// <summary>
/// Обработчик принятия приглашения.
/// Проверяет profileId текущего пользователя, создаёт OrganizationMember.
/// </summary>
public sealed class AcceptInvitationCommandHandler(IServiceProvider provider)
    : ICommandHandler<AcceptInvitationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();

        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var spec = new InvitationSpecification(request.Token);
        var invitation =
            await invitationRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Приглашение с указанным токеном не найдено.");

        // Проверить, что пользователь ещё не является участником
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var memberSpec = new OrganizationMemberSpecification(profileId, invitation.OrganizationId);
        var existingMember = await memberRepo.FindAsync(memberSpec, cancellationToken);

        if (existingMember is not null)
            throw new InvalidOperationException("Вы уже являетесь участником данной организации.");

        // Принять приглашение (domain logic: проверка статуса, TTL, profileId)
        invitation.Accept(profileId);

        // Создать участника организации
        var member = new OrganizationMember(invitation.OrganizationId, profileId, invitation.Role);

        await memberRepo.AddAsync(member, cancellationToken);
        await invitationRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
