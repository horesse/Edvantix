using Edvantix.Organizational.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.Organizational.Features.ContactFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.ContactFeature.Features.GetContacts;

public sealed record GetContactsQuery(long OrganizationId) : IRequest<IEnumerable<ContactModel>>;

public sealed class GetContactsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetContactsQuery, IEnumerable<ContactModel>>
{
    public async Task<IEnumerable<ContactModel>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new ContactSpecification { OrganizationId = request.OrganizationId };

        using var contactRepo = provider.GetRequiredService<IContactRepository>();
        var contacts = await contactRepo.GetByExpressionAsync(spec, cancellationToken);

        return contacts.Select(c => new ContactModel
        {
            Id = c.Id,
            OrganizationId = c.OrganizationId,
            Type = c.Type,
            Value = c.Value,
            Description = c.Description,
        });
    }
}
