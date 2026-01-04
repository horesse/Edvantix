using Edvantix.Chassis.CQRS.Crud.Abstractions;
using FluentValidation;

namespace Edvantix.Chassis.CQRS.Crud.Validators;

public sealed class CommandValidator<TModel> : AbstractValidator<ValidateCommand<TModel>>
    where TModel : class
{
    public CommandValidator(IValidator<TModel> modelValidator)
    {
        RuleFor(x => x.Model)
            .NotNull()
            .WithMessage("Модель обязательна.")
            .SetValidator(modelValidator);
    }
}
