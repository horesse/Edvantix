using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupFeature.Features.CreateGroup;

public sealed record CreateGroupCommand(
    long OrganizationId,
    string Name,
    string? Description
) : IRequest<long>;

public sealed class CreateGroupCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateGroupCommand, long>
{
    public async Task<long> Handle(
        CreateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager,
            OrganizationRole.Teacher
        );

        var group = new Group(request.OrganizationId, request.Name, request.Description);

        using var groupRepo = provider.GetRequiredService<IGroupRepository>();

        await groupRepo.InsertAsync(group, cancellationToken);
        await groupRepo.SaveEntitiesAsync(cancellationToken);

        return group.Id;
    }
}
