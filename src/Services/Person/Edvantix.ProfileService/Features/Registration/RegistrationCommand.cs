using System.Security.Claims;
using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.Registration;

public sealed record RegistrationCommand(
    Gender Gender,
    ProfileModel Profile,
    ICollection<UserContactModel> Contacts,
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

        using var personRepo = provider.GetRequiredService<IProfileRepository>();

        var isExists = await personRepo.AnyAsync(p => p.AccountId == userGuid, cancellationToken);

        if (isExists)
            throw new Exception("Пользователь с таким идентификатором уже существует");

        await using var transaction = await personRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var person = new Profile(
                userGuid,
                request.Gender,
                request.Profile.FirstName,
                request.Profile.LastName,
                request.Profile.MiddleName
            );

            await personRepo.InsertAsync(person, cancellationToken);

            if (request.Contacts.Count != 0)
            {
                var converter = provider.GetRequiredService<
                    IConverter<UserContactModel, UserContact>
                >();
                var contacts = converter.Map([.. request.Contacts]);

                person.AddContacts(contacts);
            }

            if (request.EmploymentHistories.Count != 0)
            {
                var converter = provider.GetRequiredService<
                    IConverter<EmploymentHistoryModel, EmploymentHistory>
                >();
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
