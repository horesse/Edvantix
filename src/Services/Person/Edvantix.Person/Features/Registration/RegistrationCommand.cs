using System.Security.Claims;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Constants.Other;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.Person.Features.ContactFeature.Mappers;
using Edvantix.Person.Features.ContactFeature.Models;
using Edvantix.Person.Features.EmploymentHistoryFeature.Mappers;
using Edvantix.Person.Features.EmploymentHistoryFeature.Models;
using Edvantix.Person.Features.PersonInfoFeature.Models;
using MediatR;

namespace Edvantix.Person.Features.Registration;

public sealed record RegistrationCommand(
    Gender Gender,
    PersonInfoModel PersonInfo,
    ICollection<ContactModel> Contacts,
    ICollection<EmploymentHistoryModel> EmploymentHistories
) : IRequest<long>;

public sealed class RegistrationCommandHandler(IServiceProvider provider)
    : IRequestHandler<RegistrationCommand, long>
{
    public async Task<long> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var claimsPrincipal =
            provider.GetService<ClaimsPrincipal>() ?? throw new Exception("Вы не авторизованы.");

        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);

        using var personRepo = provider.GetRequiredService<IPersonInfoRepository>();

        await using var transaction = await personRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var person = new PersonInfo(
                Guid.Parse(userId),
                request.Gender,
                request.PersonInfo.FirstName,
                request.PersonInfo.LastName,
                request.PersonInfo.MiddleName
            );

            await personRepo.InsertAsync(person, cancellationToken);

            if (request.Contacts.Count != 0)
            {
                var converter = provider.GetRequiredService<ContactConverter>();
                var contacts = converter.Map([.. request.Contacts]);

                using var contactRepo = provider.GetRequiredService<IContactRepository>();
                await contactRepo.InsertRangeAsync(contacts, cancellationToken);
            }

            if (request.EmploymentHistories.Count != 0)
            {
                var converter = provider.GetRequiredService<EmploymentHistoryConverter>();
                var histories = converter.Map([.. request.EmploymentHistories]);

                using var employmentRepo =
                    provider.GetRequiredService<IEmploymentHistoryRepository>();
                await employmentRepo.InsertRangeAsync(histories, cancellationToken);
            }

            await personRepo.SaveEntitiesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return person.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
