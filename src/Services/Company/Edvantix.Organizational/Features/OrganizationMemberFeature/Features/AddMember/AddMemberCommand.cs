using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.AddMember;

public sealed record AddMemberCommand(ulong OrganizationId, ulong ProfileId, OrganizationRole Role)
    : IRequest<Guid>;

public sealed class AddMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddMemberCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddMemberCommand request,
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

        // Проверить, что пользователь ещё не является участником
        var spec = new OrganizationMemberSpecification(request.ProfileId, request.OrganizationId);
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var existing = await memberRepo.FindAsync(spec, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException(
                "Пользователь уже является участником данной организации."
            );

        var member = new OrganizationMember(
            request.OrganizationId,
            request.ProfileId,
            request.Role
        );

        await memberRepo.AddAsync(member, cancellationToken);
        await memberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
