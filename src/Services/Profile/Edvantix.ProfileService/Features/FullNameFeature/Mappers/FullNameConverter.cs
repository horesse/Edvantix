using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.ProfileService.Features.FullNameFeature.Models;

namespace Edvantix.ProfileService.Features.FullNameFeature.Mappers;

public sealed class FullNameConverter : ClassConverter<FullNameModel, FullName>
{
    public override FullName Map(FullNameModel source) =>
        new(source.FirstName, source.LastName, source.MiddleName);

    public override FullNameModel Map(FullName source) =>
        new()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName,
            Id = source.Id,
        };

    public override void SetProperties(FullNameModel source, FullName target)
    {
        target.Update(source.FirstName, source.LastName, source.MiddleName);
    }
}
