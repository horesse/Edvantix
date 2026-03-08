using Edvantix.Organizational.Features.OrganizationFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetContacts;

/// <summary>
/// Запрос списка контактов организации.
/// </summary>
public sealed record GetContactsQuery(Guid OrganizationId) : IQuery<IEnumerable<ContactModel>>;

/// <summary>
/// Обработчик запроса контактов. Доступно любому участнику организации.
/// </summary>
public sealed class GetContactsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetContactsQuery, IEnumerable<ContactModel>>
{
    public async ValueTask<IEnumerable<ContactModel>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        // Загружаем организацию вместе с контактами через спецификацию
        var spec = new OrganizationSpecification(request.OrganizationId, includeContacts: true);
        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org =
            await orgRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException(
                $"Организация с ID {request.OrganizationId} не найдена."
            );

        return org.Contacts.Select(c => new ContactModel
        {
            Id = c.Id,
            OrganizationId = c.OrganizationId,
            Type = c.Type,
            Value = c.Value,
            Description = c.Description,
        });
    }
}
