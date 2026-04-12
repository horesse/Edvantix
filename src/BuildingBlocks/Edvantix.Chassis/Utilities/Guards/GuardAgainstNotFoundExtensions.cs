using System.Diagnostics.CodeAnalysis;
using Edvantix.Chassis.Exceptions;

namespace Edvantix.Chassis.Utilities.Guards;

public static class GuardAgainstNotFoundExtensions
{
    extension(Guard guard)
    {
        /// <summary>
        /// Проверяет, что переданное значение не равно null. Если null, выбрасывает <see cref="NotFoundException" />
        /// для указанного <paramref name="id" />.
        /// </summary>
        /// <typeparam name="T">Тип проверяемого значения.</typeparam>
        /// <param name="value">Значение для проверки на null.</param>
        /// <param name="id">Идентификатор, связанный с проверяемой сущностью.</param>
        /// <exception cref="NotFoundException">
        /// Выбрасывается, когда значение равно null, что означает, что сущность с указанным <paramref name="id" /> не найдена.
        /// </exception>
        public void NotFound<T>([NotNull] T? value, string id)
        {
            if (value is not null)
            {
                return;
            }

            throw NotFoundException.For<T>(id);
        }

        /// <summary>
        /// Проверяет, что переданное значение не равно null. Если null, выбрасывает <see cref="NotFoundException" />
        /// для указанного <paramref name="id" />.
        /// </summary>
        /// <typeparam name="T">Тип проверяемого значения.</typeparam>
        /// <param name="value">Значение для проверки на null.</param>
        /// <param name="id">Идентификатор, связанный с проверяемой сущностью.</param>
        /// <exception cref="NotFoundException">
        /// Выбрасывается, когда значение равно null, что означает, что сущность с указанным <paramref name="id" /> не найдена.
        /// </exception>
        public void NotFound<T>([NotNull] T? value, Guid id)
        {
            if (value is not null)
            {
                return;
            }

            throw NotFoundException.For<T>(id);
        }
    }
}
