namespace Edvantix.AppHost.Extensions.Security;

internal static class SecretExtensions
{
    extension(IResourceBuilder<ProjectResource> builder)
    {
        /// <summary>
        /// Настраивает построитель ресурса для создания сгенерированного параметра-секрета и его экспозиции как переменной среды.
        /// </summary>
        /// <param name="secretName">
        /// Суффикс для формирования имени параметра; итоговый параметр будет иметь вид
        /// <c>{builder.Resource.Name}-{secretName}</c>.
        /// </param>
        /// <param name="environmentVariableName">
        /// Имя переменной среды, которая будет привязана к созданному параметру-секрету.
        /// </param>
        /// <returns>
        /// Исходный <see cref="IResourceBuilder{ProjectResource}" /> после регистрации параметра и его сопоставления с переменной среды.
        /// </returns>
        /// <remarks>
        /// Сгенерированный параметр использует ограничения по умолчанию: минимальная длина 32, без специальных символов.
        /// </remarks>
        public IResourceBuilder<ProjectResource> WithSecret(
            string secretName,
            string environmentVariableName
        )
        {
            var secret = builder
                .ApplicationBuilder.AddParameter($"{builder.Resource.Name}-{secretName}", true)
                .WithGeneratedDefault(new() { MinLength = 32, Special = false });

            return builder.WithEnvironment(environmentVariableName, secret);
        }
    }

    extension(IResourceBuilder<ParameterResource> builder)
    {
        /// <summary>
        /// Настраивает построитель ресурса для генерации значения параметра по умолчанию и переопределения начального состояния ресурса.
        /// </summary>
        /// <param name="generateParameterDefault">
        /// Делегат для генерации значения параметра по умолчанию с заданными ограничениями.
        /// </param>
        /// <returns>
        /// Настроенный экземпляр <see cref="IResourceBuilder{ParameterResource}" /> со сгенерированным значением по умолчанию и корректным начальным состоянием.
        /// </returns>
        public IResourceBuilder<ParameterResource> WithGeneratedDefault(
            GenerateParameterDefault generateParameterDefault
        )
        {
            var generatedParameter = ParameterResourceBuilderExtensions.CreateGeneratedParameter(
                builder.ApplicationBuilder,
                builder.Resource.Name,
                builder.Resource.Secret,
                generateParameterDefault
            );

            builder.Resource.Default = generatedParameter.Default;

            builder.WithInitialState(
                new()
                {
                    ResourceType = "Parameter",
                    IsHidden = true,
                    Properties =
                    [
                        new("parameter.secret", builder.Resource.Secret.ToString()),
                        new(
                            CustomResourceKnownProperties.Source,
                            $"Parameters:{builder.Resource.Name}"
                        ),
                    ],
                }
            );

            return builder;
        }
    }
}
