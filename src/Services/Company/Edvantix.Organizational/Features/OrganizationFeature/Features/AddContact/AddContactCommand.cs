using Edvantix.Constants.Other;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.AddContact;

/// <summary>
/// Команда добавления контакта организации.
/// </summary>
public sealed record AddContactCommand(
    ulong OrganizationId,
    ContactType Type,
    string Value,
    string? Description
) : IRequest<ulong>;

/// <summary>
/// Обработчик добавления контакта. Авторизация: Owner/Manager.
/// Контакт добавляется через агрегат Organization и сохраняется единой транзакцией.
/// </summary>
public sealed class AddContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddContactCommand, ulong>
{
    public async ValueTask<ulong> Handle(
        AddContactCommand request,
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

        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org =
            await orgRepo.FindByIdAsync(request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(
                $"Организация с ID {request.OrganizationId} не найдена."
            );

        // AddContact добавляет в коллекцию _contacts — EF Core отследит новый объект
        var contact = org.AddContact(request.Type, request.Value, request.Description);

        await orgRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return contact.Id;
    }
}
