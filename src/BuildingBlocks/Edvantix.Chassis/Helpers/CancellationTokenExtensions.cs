using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Chassis.Helpers;

public static class CancellationTokenExtensions
{
    /// <summary>
    /// Проверяет, был ли вызван запрос на отмену для переданного CancellationToken.
    /// Если отмена произошла, возвращает true и возвращает связанное исключение.
    /// </summary>
    /// <param name="token">CancellationToken для проверки запросов на отмену.</param>
    /// <param name="exception">Исключение, связанное с отменой, если оно есть.</param>
    /// <returns>
    /// True, если обнаружен запрос на отмену; в противном случае — false.
    /// </returns>
    public static bool GetErrorIfCancellationRequested(
        this CancellationToken token,
        [MaybeNullWhen(false)]
        out Exception exception
    )
    {
        exception = null;

        if (token.IsCancellationRequested)
        {
            exception = new TaskCanceledException();

            return true;
        }
        
        return false;
    }
}
