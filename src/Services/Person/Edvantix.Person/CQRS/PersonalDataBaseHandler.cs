using System.Security.Claims;
using Edvantix.Chassis.CQRS.Crud.Handlers;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Person.Domain.Abstractions;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Person.CQRS;

public class PersonalDataBaseHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider)
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    private readonly IServiceProvider _provider = provider;
    private string? Sub => ClaimsPrincipal?.GetClaimValue(ClaimTypes.NameIdentifier);

    protected async Task SetPersonInfoId(
        PersonalData<TIdentity> personalData,
        CancellationToken cancellationToken
    )
    {
        var userId = Guard.Against.NotAuthenticated(Sub);
        using var repo = _provider.GetRequiredService<IPersonInfoRepository>();

        var personInfoId = await repo.GetAsQueryable()
            .AsNoTracking()
            .Where(x => x.AccountId.ToString() == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (personInfoId == 0)
            throw new Exception("Профиль не найден.");

        if (personalData.PersonInfoId > 0 && personalData.PersonInfoId != personInfoId)
            throw new Exception("У Вас нет прав на изменение данной сущности.");

        personalData.SetPersonInfoId(personInfoId);
    }
}
