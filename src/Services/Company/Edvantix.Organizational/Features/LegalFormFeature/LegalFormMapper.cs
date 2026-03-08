using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;
using Edvantix.Organizational.Features.LegalFormFeature.Models;

namespace Edvantix.Organizational.Features.LegalFormFeature;

/// <summary>
/// Маппер для преобразования <see cref="LegalForm"/> в <see cref="LegalFormModel"/>.
/// </summary>
public sealed class LegalFormMapper : Mapper<LegalForm, LegalFormModel>
{
    public override LegalFormModel Map(LegalForm source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            ShortName = source.ShortName,
        };
}
