using Edvantix.SharedKernel.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Http.FluentValidation;
using Wolverine.Persistence;

namespace Edvantix.Chassis.CQRS;

public static class Extensions
{
    /// <summary>
    /// Имя ActivitySource и Meter, которое Wolverine регистрирует для OpenTelemetry.
    /// Используется в ServiceDefaults для подключения трассировки и метрик.
    /// </summary>
    public const string WolverineActivitySourceName = "Wolverine";

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует Wolverine с автообнаружением обработчиков из сборки <typeparamref name="TMarker"/>
        /// и стандартными политиками логирования. Дополнительная конфигурация передаётся через <paramref name="configure"/>.
        /// </summary>
        public IServiceCollection AddWolverine<TMarker>(Action<WolverineOptions>? configure = null)
        {
            services.AddWolverine(
                ExtensionDiscovery.ManualOnly,
                opts =>
                {
                    opts.Discovery.IncludeAssembly(typeof(TMarker).Assembly);

                    opts.Policies.LogMessageStarting(LogLevel.Information);
                    opts.Policies.MessageExecutionLogLevel(LogLevel.None);
                    opts.Policies.MessageSuccessLogLevel(LogLevel.Debug);

                    configure?.Invoke(opts);
                }
            );

            return services;
        }
    }

    extension(WolverineOptions options)
    {
        /// <summary>
        /// Включает публикацию доменных событий через EF Core и опционально оборачивает обработчики
        /// в транзакцию EF Core.
        /// </summary>
        public void UseDomainEvents(bool useTransactions = true)
        {
            if (useTransactions)
            {
                options.UseEntityFrameworkCoreTransactions();
            }

            options.PublishDomainEventsFromEntityFrameworkCore<HasDomainEvents>(x =>
                x.DomainEvents
            );
        }

        /// <summary>
        /// Подключает FluentValidation к конвейеру Wolverine: валидация сообщений и HTTP ProblemDetails.
        /// </summary>
        public void UseValidation()
        {
            options.UseFluentValidation(fv => fv.IncludeInternalTypes = true);
            options.UseFluentValidationProblemDetail();
        }

        /// <summary>
        /// Включает полный режим трассировки OpenTelemetry для <c>IMessageBus.InvokeAsync</c>.
        /// По умолчанию Wolverine использует <c>InvokeTracingMode.Lightweight</c>.
        /// <c>Full</c> эмитирует те же структурированные события, что и асинхронные обработчики.
        /// </summary>
        public void UseFullOpenTelemetry()
        {
            options.InvokeTracing = InvokeTracingMode.Full;
        }

        /// <summary>
        /// Автоматически оборачивает обработчики сообщений в транзакцию EF Core,
        /// если обработчик принимает <c>DbContext</c> как параметр.
        /// Альтернатива явному вызову <see cref="UseDomainEvents"/> с транзакциями.
        /// </summary>
        public void UseAutoTransactions(
            IdempotencyStyle idempotency = IdempotencyStyle.None
        )
        {
            options.Policies.AutoApplyTransactions(idempotency);
        }
    }
}
