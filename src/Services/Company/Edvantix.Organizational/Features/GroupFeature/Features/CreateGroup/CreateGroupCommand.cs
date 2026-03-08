using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.CreateGroup;

public sealed record CreateGroupCommand(Guid OrganizationId, string Name, string? Description)
    : ICommand<Guid>;

public sealed class CreateGroupCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateGroupCommand, Guid>
{
    public async ValueTask<Guid> Handle(
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

        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        await groupRepo.AddAsync(group, cancellationToken);
        await groupRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return group.Id;
    }
}
