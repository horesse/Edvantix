using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.DeleteContact;

/// <summary>
/// Команда удаления контакта организации.
/// </summary>
public sealed record DeleteContactCommand(ulong OrganizationId, ulong ContactId) : IRequest<Unit>;

/// <summary>
/// Обработчик удаления контакта. Авторизация: Owner/Manager.
/// Удаление выполняется через агрегат Organization.
/// </summary>
public sealed class DeleteContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteContactCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        DeleteContactCommand request,
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

        // Загружаем организацию с контактами, чтобы найти нужный
        var spec = new OrganizationSpecification(request.OrganizationId, includeContacts: true);
        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org =
            await orgRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException(
                $"Организация с ID {request.OrganizationId} не найдена."
            );

        // Агрегат выбрасывает InvalidOperationException если контакт не найден
        org.RemoveContact(request.ContactId);

        await orgRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
