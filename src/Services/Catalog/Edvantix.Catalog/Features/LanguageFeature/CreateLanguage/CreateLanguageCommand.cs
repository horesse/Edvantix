namespace Edvantix.Catalog.Features.LanguageFeature.CreateLanguage;

using Mediator;

/// <summary>
/// Команда создания языка.
/// </summary>
public sealed record CreateLanguageCommand(
    string Code,
    string NameRu,
    string NameEn,
    string NativeName
) : ICommand<LanguageModel>;

/// <summary>
/// Обработчик <see cref="CreateLanguageCommand"/>.
/// HTTP 409 — если язык с таким кодом уже существует.
/// </summary>
public sealed class CreateLanguageCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateLanguageCommand, LanguageModel>
{
    /// <inheritdoc/>
    public async ValueTask<LanguageModel> Handle(
        CreateLanguageCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ILanguageRepository>();
        var mapper = provider.GetRequiredService<IMapper<Language, LanguageModel>>();

        var existing = await repo.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Язык с кодом '{request.Code.ToLowerInvariant()}' уже существует."
            );
        }

        var language = new Language(
            request.Code,
            request.NameRu,
            request.NameEn,
            request.NativeName
        );

        await repo.AddAsync(language, cancellationToken);
        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(language);
    }
}
