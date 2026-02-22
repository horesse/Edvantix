using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.DeleteGroup;

public sealed record DeleteGroupCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteGroupCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteGroupCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        DeleteGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group =
            await groupRepo.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            group.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        // Soft-delete: cascades to GroupMembers via domain model
        group.Delete();
        await groupRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
