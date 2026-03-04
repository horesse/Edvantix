namespace Edvantix.Catalog.Features.LanguageFeature.UpdateLanguage;

/// <summary>
/// Команда обновления языка. Требует передачи всех изменяемых полей.
/// <para>
/// Логика IsActive: <c>false</c> → деактивация; <c>true</c> → активация; <c>null</c> → без изменений.
/// </para>
/// </summary>
public sealed record UpdateLanguageCommand(
    string Code,
    string NameRu,
    string NameEn,
    string NativeName,
    bool? IsActive
) : IRequest<LanguageModel>;

/// <summary>
/// Обработчик <see cref="UpdateLanguageCommand"/>.
/// HTTP 404 — если язык не найден.
/// </summary>
public sealed class UpdateLanguageCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateLanguageCommand, LanguageModel>
{
    /// <inheritdoc/>
    public async ValueTask<LanguageModel> Handle(
        UpdateLanguageCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ILanguageRepository>();
        var mapper = provider.GetRequiredService<IMapper<Language, LanguageModel>>();

        var language =
            await repo.FindTrackedByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Язык с кодом '{request.Code}' не найден.");

        if (request.IsActive == false && language.IsActive)
        {
            language.Deactivate();
        }
        else if (request.IsActive == true && !language.IsActive)
        {
            language.Activate();
        }

        language.Update(request.NameRu, request.NameEn, request.NativeName);

        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(language);
    }
}
