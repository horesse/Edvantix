using Edvantix.Chassis.CQRS.Crud.Abstractions;
using FluentValidation;

namespace Edvantix.Chassis.CQRS.Crud.Validators;

public sealed class UpdateCommandValidator<TModel, TIdentity>
    : AbstractValidator<UpdateCommand<TModel, TIdentity>>
    where TModel : class
    where TIdentity : struct
{
    public UpdateCommandValidator(IValidator<TModel> modelValidator)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор обязателен.");

        RuleFor(x => x.Model)
            .NotNull()
            .WithMessage("Модель обязательна.")
            .SetValidator(modelValidator);
    }
}
