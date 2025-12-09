using System.Reflection;
using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.CQRS.Pipelines;

public sealed class ActivityBehavior<TMessage, TResponse>(
    IActivityScope activityScope,
    CommandHandlerMetrics commandMetrics,
    QueryHandlerMetrics queryMetrics,
    ILogger<ActivityBehavior<TMessage, TResponse>> logger
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull
{
    public async Task<TResponse> Handle(
        TMessage message,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "[{Behavior}] handle request={RequestData} and response={ResponseData}",
                nameof(ActivityBehavior<,>),
                message.GetType().Name,
                typeof(TResponse).Name
            );
        }

        var attr = message.GetType().GetCustomAttribute<IgnoreOTelOnHandlerAttribute>();

        if (attr is not null)
        {
            return await next(cancellationToken);
        }

        var messageType = message.GetType().Name;
        var handlerName = $"{messageType}Handler";
        var activityName = $"{handlerName}/{messageType}";

        var isCommand = messageType.ToLowerInvariant().EndsWith(nameof(Command).ToLowerInvariant());
        var tagName = isCommand ? TelemetryTags.Commands.Command : TelemetryTags.Queries.Query;

        var startingTimestamp = isCommand
            ? commandMetrics.CommandHandlingStart(handlerName)
            : queryMetrics.QueryHandlingStart(handlerName);

        try
        {
            return await activityScope.Run(
                activityName,
                async (_, ct) => await next(ct),
                new() { Tags = { { tagName, messageType } } },
                cancellationToken
            );
        }
        finally
        {
            if (isCommand)
            {
                commandMetrics.CommandHandlingEnd(handlerName, startingTimestamp);
            }
            else
            {
                queryMetrics.QueryHandlingEnd(handlerName, startingTimestamp);
            }
        }
    }
}
