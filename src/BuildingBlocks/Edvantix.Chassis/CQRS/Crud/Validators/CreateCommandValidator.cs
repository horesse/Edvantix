using Edvantix.Chassis.CQRS.Crud.Abstractions;
using FluentValidation;

namespace Edvantix.Chassis.CQRS.Crud.Validators;

public sealed class CreateCommandValidator<TModel, TIdentity> : AbstractValidator<CreateCommand<TModel, TIdentity>>
    where TModel : class
    where TIdentity : struct
{
    public CreateCommandValidator(IValidator<TModel> modelValidator)
    {
        RuleFor(x => x.Model)
            .NotNull()
            .WithMessage("Модель обязательна.")
            .SetValidator(modelValidator);
    }
}
