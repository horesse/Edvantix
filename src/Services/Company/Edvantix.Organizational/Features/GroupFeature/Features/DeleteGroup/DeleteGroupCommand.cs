using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.DeleteGroup;

public sealed record DeleteGroupCommand(long Id) : IRequest<Unit>;

public sealed class DeleteGroupCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteGroupCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            group.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        await groupRepo.DeleteAsync(group, cancellationToken);
        await groupRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
