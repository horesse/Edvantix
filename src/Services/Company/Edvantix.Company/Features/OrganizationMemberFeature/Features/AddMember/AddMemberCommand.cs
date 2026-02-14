using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.AddMember;

public sealed record AddMemberCommand(
    long OrganizationId,
    int ProfileId,
    OrganizationRole Role
) : IRequest<Guid>;

public sealed class AddMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddMemberCommand, Guid>
{
    public async Task<Guid> Handle(
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
        var spec = new OrganizationMemberByProfileSpecification(
            request.ProfileId,
            request.OrganizationId
        );

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var existing = await memberRepo.GetFirstByExpressionAsync(spec, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException(
                "Пользователь уже является участником данной организации."
            );

        var member = new OrganizationMember(
            request.OrganizationId,
            request.ProfileId,
            request.Role
        );

        await memberRepo.InsertAsync(member, cancellationToken);
        await memberRepo.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
