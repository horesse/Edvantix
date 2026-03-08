namespace Edvantix.Catalog.Features.RegionFeature.CreateRegion;

using Mediator;

/// <summary>
/// Команда создания региона.
/// </summary>
public sealed record CreateRegionCommand(string Code, string NameRu, string NameEn)
    : ICommand<RegionModel>;

/// <summary>
/// Обработчик <see cref="CreateRegionCommand"/>.
/// HTTP 409 — если регион с таким кодом уже существует.
/// </summary>
public sealed class CreateRegionCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateRegionCommand, RegionModel>
{
    /// <inheritdoc/>
    public async ValueTask<RegionModel> Handle(
        CreateRegionCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IRegionRepository>();
        var mapper = provider.GetRequiredService<IMapper<Region, RegionModel>>();

        var existing = await repo.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Регион с кодом '{request.Code.ToUpperInvariant()}' уже существует."
            );
        }

        var region = new Region(request.Code, request.NameRu, request.NameEn);

        await repo.AddAsync(region, cancellationToken);
        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(region);
    }
}
