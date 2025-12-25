using System.Security.Claims;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Person.CQRS.Pipelines;

/// <summary>
/// Pipeline behavior для автоматического заполнения PersonInfoId в командах работы с PersonalData.
/// Извлекает userId из ClaimsPrincipal и находит соответствующий PersonInfo.
/// </summary>
public sealed class PersonalDataBehavior<TRequest, TResponse>(
    ClaimsPrincipal claimsPrincipal,
    IPersonInfoRepository personInfoRepository,
    ILogger<PersonalDataBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // Проверяем, является ли команда PersonalData командой
        if (request is not IPersonalDataCommand personalDataCommand)
        {
            return await next();
        }

        // Если PersonInfoId уже заполнен, пропускаем
        if (personalDataCommand.PersonInfoId != 0)
        {
            logger.LogDebug(
                "PersonInfoId already set to {PersonInfoId}, skipping automatic population",
                personalDataCommand.PersonInfoId
            );
            return await next();
        }

        // Извлекаем userId из claims
        var sub = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
        var userId = Guard.Against.NotAuthenticated(sub);

        logger.LogDebug("Resolving PersonInfoId for userId: {UserId}", userId);

        // Находим PersonInfo по AccountId
        var personInfoId = await personInfoRepository
            .GetAsQueryable()
            .AsNoTracking()
            .Where(x => x.AccountId.ToString() == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(personInfoId, userId);

        // Заполняем PersonInfoId
        personalDataCommand.PersonInfoId = personInfoId;

        logger.LogInformation(
            "Automatically populated PersonInfoId={PersonInfoId} for user={UserId}",
            personInfoId,
            userId
        );

        return await next();
    }
}
