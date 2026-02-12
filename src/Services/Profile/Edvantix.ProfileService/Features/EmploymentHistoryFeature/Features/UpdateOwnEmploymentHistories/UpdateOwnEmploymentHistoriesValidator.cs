using FluentValidation;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature.Features.UpdateOwnEmploymentHistories;

/// <summary>
/// Валидатор команды обновления истории трудоустройства
/// </summary>
public sealed class UpdateOwnEmploymentHistoriesValidator
    : AbstractValidator<UpdateOwnEmploymentHistoriesCommand>
{
    public UpdateOwnEmploymentHistoriesValidator()
    {
        RuleForEach(x => x.EmploymentHistories).SetValidator(new Validator());
    }
}
