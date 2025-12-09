using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.CQRS.Pipelines;

/// <summary>
/// Pipeline behavior для валидации запросов с помощью FluentValidation
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IActivityScope activityScope,
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        const string behavior = nameof(ValidationBehavior<,>);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "[{Behavior}] handle request={RequestData} and response={ResponseData}",
                behavior,
                typeof(TRequest).Name,
                typeof(TResponse).Name
            );
        }

        // Если валидаторов нет, пропускаем валидацию
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Запускаем все валидаторы параллельно
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Собираем все ошибки валидации
        var errors = validationResults
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(failure => new ValidationFailure(failure.PropertyName, failure.ErrorMessage))
            .ToList();

        if (errors.Count != 0)
        {
            throw new ValidationException(errors);
        }

        // Трейсинг валидации
        var requestType = typeof(TRequest).Name;
        var validatorNames = validators.Aggregate("", (c, x) => $"{x.GetType().Name}, {c}");
        var activityName = $"{requestType}-{validatorNames.Trim().TrimEnd(',')}";

        await activityScope.Run(
            activityName,
            (_, _) => Task.CompletedTask,
            new() { Tags = { { TelemetryTags.Validator.Validation, requestType } } },
            cancellationToken
        );

        return await next();
    }
}
