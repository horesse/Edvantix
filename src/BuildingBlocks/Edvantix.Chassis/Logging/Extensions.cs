using System.Text;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.Logging;

public static class Extensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Настраивает сервисы редактирования логов для построителя хоста.
        /// </summary>
        /// <remarks>
        /// Метод включает редактирование логов, применяет <see cref="AsteriskRedactor" /> для чувствительных данных
        /// и настраивает HMAC-редактирование для персональных данных на основе значения `HMAC:Key`.
        /// Ключ кодируется в UTF-8 и конвертируется в Base64, так как HMAC-редактор ожидает ключ в формате Base64.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, когда значение конфигурации `HMAC:Key` отсутствует или пустое.
        /// </exception>
        public void AddRedaction()
        {
            var keyString = builder
                .Configuration.GetRequiredSection("HMAC")
                .GetValue<string>("Key")
                ?.Trim();

            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("HMAC key configuration is missing or empty");
            }

            // Конвертирует ключевой материал в Base64 для конфигурации HMAC-редактора.
            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            var base64Key = Convert.ToBase64String(keyBytes);

            // Включает поддержку редактирования на уровне фреймворка в логировании.
            builder.Logging.EnableRedaction();

            // Регистрирует редакторы по классификации данных.
            builder.Services.AddRedaction(x =>
            {
                // Маскирует чувствительные поля звёздочками.
                x.SetRedactor<AsteriskRedactor>(
                    new DataClassificationSet(DataTaxonomy.SensitiveData)
                );

                // Применяет детерминированное HMAC-редактирование для полей персональных данных.
                x.SetHmacRedactor(
                    options =>
                    {
                        options.KeyId = 10;
                        options.Key = base64Key;
                    },
                    new DataClassificationSet(DataTaxonomy.PIIData)
                );
            });
        }

        /// <summary>
        /// Регистрирует <see cref="ApplicationEnricher" /> для обогащения событий лога свойствами приложения,
        /// такими как имя машины и идентификатор пользователя.
        /// </summary>
        public void AddApplicationEnricher()
        {
            builder.Services.AddLogEnricher<ApplicationEnricher>();
        }
    }
}
