using Edvantix.Chassis.CQRS.Crud.Abstractions;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

/// <summary>
/// Handler для валидации модели
/// </summary>
public class ValidateCommandHandler<TModel>
    : IRequestHandler<ValidateCommand<TModel>, bool>
    where TModel : class
{
    public virtual Task<bool> Handle(
        ValidateCommand<TModel> command,
        CancellationToken cancellationToken
    )
    {
        // По умолчанию возвращаем true
        // Валидация через FluentValidation происходит в ValidationBehavior pipeline
        // Этот метод может быть переопределен для кастомной логики
        return Task.FromResult(true);
    }
}
