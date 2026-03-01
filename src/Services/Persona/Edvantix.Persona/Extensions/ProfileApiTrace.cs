using Edvantix.Persona.Domain.Events;

namespace Edvantix.Persona.Extensions;

internal static partial class ProfileApiTrace
{
    [LoggerMessage(
        EventId = 0,
        EventName = nameof(ProfileRegisteredEvent),
        Level = LogLevel.Information,
        Message = "Зарегистрирован аккаунт {AccountId} с логином {login}"
    )]
    public static partial void LogProfileRegistered(
        ILogger logger,
        [SensitiveData] Guid accountId,
        [SensitiveData] string login
    );
}
