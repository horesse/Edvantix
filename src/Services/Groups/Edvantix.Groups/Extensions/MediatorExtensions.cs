using Edvantix.Groups.Pipelines;

namespace Edvantix.Groups.Extensions;

internal static class MediatorExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует <see cref="AuthorizationBehavior{TMessage,TResponse}"/> в конвейере Mediator.
        /// Проверяет разрешения профиля в организации для команд и запросов с атрибутом <c>[RequirePermission]</c>
        /// через gRPC-вызов к Organizational.
        /// </summary>
        internal IServiceCollection ApplyAuthorizationBehavior()
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            return services;
        }
    }
}
