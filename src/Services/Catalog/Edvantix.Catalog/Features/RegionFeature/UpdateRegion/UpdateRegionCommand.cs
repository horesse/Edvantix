namespace Edvantix.Catalog.Features.RegionFeature.UpdateRegion;

/// <summary>
/// Команда обновления региона. Требует передачи всех изменяемых полей.
/// <para>
/// Логика IsActive: <c>false</c> → деактивация; <c>true</c> → активация; <c>null</c> → без изменений.
/// </para>
/// </summary>
public sealed record UpdateRegionCommand(string Code, string NameRu, string NameEn, bool? IsActive)
    : IRequest<RegionModel>;

/// <summary>
/// Обработчик <see cref="UpdateRegionCommand"/>.
/// HTTP 404 — если регион не найден.
/// </summary>
public sealed class UpdateRegionCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateRegionCommand, RegionModel>
{
    /// <inheritdoc/>
    public async ValueTask<RegionModel> Handle(
        UpdateRegionCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IRegionRepository>();
        var mapper = provider.GetRequiredService<IMapper<Region, RegionModel>>();

        var region =
            await repo.FindTrackedByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Регион с кодом '{request.Code}' не найден.");

        if (request.IsActive == false && region.IsActive)
        {
            region.Deactivate();
        }
        else if (request.IsActive == true && !region.IsActive)
        {
            region.Activate();
        }

        region.Update(request.NameRu, request.NameEn);

        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(region);
    }
}
