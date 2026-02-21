using Edvantix.Constants.Other;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.UpdateContact;

/// <summary>
/// Команда обновления контакта организации.
/// </summary>
public sealed record UpdateContactCommand(
    ulong OrganizationId,
    ulong ContactId,
    ContactType Type,
    string Value,
    string? Description
) : IRequest<Unit>;

/// <summary>
/// Обработчик обновления контакта. Авторизация: Owner/Manager.
/// Обновление выполняется через агрегат Organization.
/// </summary>
public sealed class UpdateContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateContactCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateContactCommand request,
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

        // Агрегат обновляет контакт и выбрасывает InvalidOperationException если не найден
        org.UpdateContact(request.ContactId, request.Type, request.Value, request.Description);

        await orgRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
