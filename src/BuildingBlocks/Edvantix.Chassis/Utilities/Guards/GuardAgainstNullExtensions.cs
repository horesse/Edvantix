using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Chassis.Utilities.Guards;

public static class GuardAgainstNullExtensions
{
    extension(Guard guard)
    {
        /// <summary>
        /// Проверяет, что переданное значение не равно null. Если null, выбрасывает <see cref="ArgumentNullException" />.
        /// </summary>
        /// <typeparam name="T">Тип проверяемого значения.</typeparam>
        /// <param name="value">Значение для проверки на null.</param>
        /// <param name="parameterName">Имя параметра, используемое в сообщении об ошибке.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, когда значение равно null.</exception>
        public void Null<T>([NotNull] T? value, string parameterName)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(value, parameterName);
        }
    }
}
