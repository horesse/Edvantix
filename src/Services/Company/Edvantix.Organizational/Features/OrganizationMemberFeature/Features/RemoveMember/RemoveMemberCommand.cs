using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.RemoveMember;

public sealed record RemoveMemberCommand(ulong OrganizationId, Guid MemberId) : IRequest<Unit>;

public sealed class RemoveMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<RemoveMemberCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        RemoveMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var member =
            await memberRepo.FindByIdAsync(request.MemberId, cancellationToken)
            ?? throw new NotFoundException($"Участник с ID {request.MemberId} не найден.");

        if (member.OrganizationId != request.OrganizationId)
            throw new NotFoundException($"Участник с ID {request.MemberId} не найден.");

        if (member.Role == OrganizationRole.Owner)
            throw new ForbiddenException("Невозможно удалить владельца организации.");

        member.Delete();
        await memberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
