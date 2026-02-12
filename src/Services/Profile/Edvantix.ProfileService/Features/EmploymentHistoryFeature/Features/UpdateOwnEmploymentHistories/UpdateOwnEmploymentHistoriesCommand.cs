using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature.Features.UpdateOwnEmploymentHistories;

/// <summary>
/// Команда для обновления истории трудоустройства текущего пользователя
/// </summary>
public sealed record UpdateOwnEmploymentHistoriesCommand(
    IEnumerable<EmploymentHistoryModel> EmploymentHistories
) : IRequest<Unit>;

/// <summary>
/// Обработчик команды обновления истории трудоустройства текущего пользователя
/// </summary>
public sealed class UpdateOwnEmploymentHistoriesCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOwnEmploymentHistoriesCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateOwnEmploymentHistoriesCommand request,
        CancellationToken cancellationToken
    )
    {
        var userGuid = provider.GetUserId();
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid, true);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var converter = provider.GetRequiredService<
                IConverter<EmploymentHistoryModel, EmploymentHistory>
            >();
            var employmentHistories = converter.Map(request.EmploymentHistories.ToList());
            profile.ReplaceEmploymentHistories(employmentHistories);

            await profileRepo.UpdateAsync(profile, cancellationToken);
            await profileRepo.SaveEntitiesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
