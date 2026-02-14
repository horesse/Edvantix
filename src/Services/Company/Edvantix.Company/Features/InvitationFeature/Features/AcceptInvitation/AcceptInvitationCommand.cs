using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.AcceptInvitation;

/// <summary>
/// Команда принятия приглашения по токену.
/// </summary>
public sealed record AcceptInvitationCommand(Guid Token) : IRequest<Guid>;

/// <summary>
/// Обработчик принятия приглашения.
/// Проверяет profileId текущего пользователя, создаёт OrganizationMember.
/// </summary>
public sealed class AcceptInvitationCommandHandler(IServiceProvider provider)
    : IRequestHandler<AcceptInvitationCommand, Guid>
{
    public async Task<Guid> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();

        var spec = new InvitationByTokenSpecification(request.Token);
        var invitation = await invitationRepo.GetFirstByExpressionAsync(spec, cancellationToken)
            ?? throw new NotFoundException($"Приглашение с указанным токеном не найдено.");

        // Проверить, что пользователь ещё не является участником.
        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var memberSpec = new OrganizationMemberByProfileSpecification(
            profileId,
            invitation.OrganizationId
        );

        var existingMember = await memberRepo.GetFirstByExpressionAsync(
            memberSpec,
            cancellationToken
        );

        if (existingMember is not null)
            throw new InvalidOperationException(
                "Вы уже являетесь участником данной организации."
            );

        // Принять приглашение (domain logic: проверка статуса, TTL, profileId).
        invitation.Accept(profileId);

        // Создать участника организации (inline, по аналогии с AddMemberCommand).
        var member = new OrganizationMember(
            invitation.OrganizationId,
            profileId,
            invitation.Role
        );

        await memberRepo.InsertAsync(member, cancellationToken);
        await invitationRepo.UpdateAsync(invitation, cancellationToken);
        await invitationRepo.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
