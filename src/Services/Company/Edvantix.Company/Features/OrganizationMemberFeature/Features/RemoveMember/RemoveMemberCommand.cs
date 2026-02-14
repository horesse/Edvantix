using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.RemoveMember;

public sealed record RemoveMemberCommand(long OrganizationId, Guid MemberId) : IRequest<Unit>;

public sealed class RemoveMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<RemoveMemberCommand, Unit>
{
    public async Task<Unit> Handle(
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

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var member = await memberRepo.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null || member.OrganizationId != request.OrganizationId)
            throw new NotFoundException($"Участник с ID {request.MemberId} не найден.");

        if (member.Role == OrganizationRole.Owner)
            throw new ForbiddenException("Невозможно удалить владельца организации.");

        await memberRepo.DeleteAsync(member, cancellationToken);
        await memberRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
