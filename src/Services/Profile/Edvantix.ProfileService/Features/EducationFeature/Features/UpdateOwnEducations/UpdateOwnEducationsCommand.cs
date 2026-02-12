using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.EducationFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.EducationFeature.Features.UpdateOwnEducations;

/// <summary>
/// Команда для обновления образования текущего пользователя
/// </summary>
public sealed record UpdateOwnEducationsCommand(IEnumerable<EducationModel> Educations)
    : IRequest<Unit>;

/// <summary>
/// Обработчик команды обновления образования текущего пользователя
/// </summary>
public sealed class UpdateOwnEducationsCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOwnEducationsCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateOwnEducationsCommand request,
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
            var converter = provider.GetRequiredService<IConverter<EducationModel, Education>>();
            var educations = converter.Map(request.Educations.ToList());
            profile.ReplaceEducations(educations);

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
