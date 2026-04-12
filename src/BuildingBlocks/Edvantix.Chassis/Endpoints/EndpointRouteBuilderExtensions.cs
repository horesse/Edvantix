using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    extension(RouteHandlerBuilder builder)
    {
        /// <summary>
        /// Настраивает метаданные ответа для POST-эндпоинта, возвращающего созданный ресурс.
        /// </summary>
        /// <typeparam name="T">
        /// Тип полезной нагрузки ответа.
        /// </typeparam>
        /// <param name="hasValidation">
        /// <see langword="true" /> для включения метаданных об ошибке валидации; иначе <see langword="false" />.
        /// </param>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesPost<T>(bool hasValidation = true)
        {
            builder = builder.Produces<T>(StatusCodes.Status201Created);

            if (hasValidation)
            {
                builder = builder.ProducesValidationProblem();
            }

            return builder;
        }

        /// <summary>
        /// Настраивает метаданные ответа для POST-эндпоинта, возвращающего успешный ответ без заголовка Location.
        /// </summary>
        /// <typeparam name="T">
        /// Тип полезной нагрузки ответа.
        /// </typeparam>
        /// <param name="hasValidation">
        /// <see langword="true" /> для включения метаданных об ошибке валидации; иначе <see langword="false" />.
        /// </param>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesPostWithoutLocation<T>(bool hasValidation = true)
        {
            builder = builder.Produces<T>();

            if (hasValidation)
            {
                builder = builder.ProducesValidationProblem();
            }

            return builder;
        }

        /// <summary>
        /// Настраивает метаданные ответа для PUT-эндпоинта.
        /// </summary>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesPut()
        {
            return builder
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesValidationProblem();
        }

        /// <summary>
        /// Настраивает метаданные ответа для DELETE-эндпоинта.
        /// </summary>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesDelete()
        {
            return builder
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Настраивает метаданные ответа для GET-эндпоинта.
        /// </summary>
        /// <typeparam name="T">
        /// Тип полезной нагрузки ответа.
        /// </typeparam>
        /// <param name="hasValidation">
        /// <see langword="true" /> для включения метаданных об ошибке валидации; иначе <see langword="false" />.
        /// </param>
        /// <param name="hasNotFound">
        /// <see langword="true" /> для включения метаданных об ошибке «не найдено»; иначе <see langword="false" />.
        /// </param>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesGet<T>(
            bool hasValidation = false,
            bool hasNotFound = false
        )
        {
            builder = builder.Produces<T>();

            if (hasValidation)
            {
                builder = builder.ProducesValidationProblem();
            }

            if (hasNotFound)
            {
                builder = builder.ProducesProblem(StatusCodes.Status404NotFound);
            }

            return builder;
        }

        /// <summary>
        /// Настраивает метаданные ответа для PATCH-эндпоинта.
        /// </summary>
        /// <typeparam name="T">
        /// Тип полезной нагрузки ответа.
        /// </typeparam>
        /// <param name="hasValidation">
        /// <see langword="true" /> для включения метаданных об ошибке валидации; иначе <see langword="false" />.
        /// </param>
        /// <returns>
        /// Настроенный построитель обработчика маршрута.
        /// </returns>
        public RouteHandlerBuilder ProducesPatch<T>(bool hasValidation = true)
        {
            builder = builder.Produces<T>().ProducesProblem(StatusCodes.Status404NotFound);

            if (hasValidation)
            {
                builder = builder.ProducesValidationProblem();
            }

            return builder;
        }
    }
}
