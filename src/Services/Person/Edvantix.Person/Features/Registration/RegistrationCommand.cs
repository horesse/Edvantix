using System.Security.Claims;
using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Constants.Other;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.Person.Features.ContactFeature.Models;
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

        var userGuid = Guid.Parse(userId);
        
        using var personRepo = provider.GetRequiredService<IPersonInfoRepository>();
        
        var isExists = await personRepo.AnyAsync(p => p.AccountId == userGuid, cancellationToken);

        if (isExists)
            throw new Exception("Пользователь с таким идентификатором уже существует");
        
        await using var transaction = await personRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var person = new PersonInfo(
                userGuid,
                request.Gender,
                request.PersonInfo.FirstName,
                request.PersonInfo.LastName,
                request.PersonInfo.MiddleName
            );

            await personRepo.InsertAsync(person, cancellationToken);

            if (request.Contacts.Count != 0)
            {
                var converter = provider.GetRequiredService<IConverter<ContactModel, Contact>>();
                var contacts = converter.Map([.. request.Contacts]);
                
                person.AddContacts(contacts);
            }

            if (request.EmploymentHistories.Count != 0)
            {
                var converter = provider.GetRequiredService<IConverter<EmploymentHistoryModel, EmploymentHistory>>();
                var histories = converter.Map([.. request.EmploymentHistories]);

                person.AddEmploymentHistories(histories);
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
