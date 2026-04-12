namespace Edvantix.Chassis.Utilities.Guards;

public static class GuardAgainstAuthenticationExtensions
{
    extension(Guard guard)
    {
        /// <summary>
        /// Проверяет, что переданный идентификатор пользователя не равен null, не пустой и не состоит только из пробелов,
        /// что указывает на аутентифицированного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя для проверки.</param>
        /// <returns>Проверенный идентификатор пользователя, если проверка аутентификации пройдена.</returns>
        /// <exception cref="UnauthorizedAccessException">Выбрасывается, когда идентификатор пользователя равен null, пустой или состоит только из пробелов.</exception>
        public string NotAuthenticated(string? userId)
        {
            return string.IsNullOrWhiteSpace(userId)
                ? throw new UnauthorizedAccessException("User is not authenticated.")
                : userId;
        }
    }
}
