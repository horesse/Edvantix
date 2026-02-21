using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    ulong Id,
    string Name,
    string NameLatin,
    string ShortName,
    string? PrintName,
    string? Description
) : IRequest<Unit>;

public sealed class UpdateOrganizationCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOrganizationCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.Id,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org =
            await orgRepo.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Организация с ID {request.Id} не найдена.");

        org.UpdateNames(request.Name, request.NameLatin, request.ShortName, request.PrintName);
        org.UpdateDescription(request.Description);

        await orgRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
