using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Edvantix.ProfileService.Features.EducationFeature.Models;

namespace Edvantix.ProfileService.Features.EducationFeature.Mappers;

public sealed class EducationMapper : ClassConverter<EducationModel, Education>
{
    public override EducationModel Map(Education source) =>
        new()
        {
            DateStart = source.DateStart,
            DateEnd = source.DateEnd,
            EducationLevelId = source.EducationLevelId,
            Specialty = source.Specialty,
            Institution = source.Institution,
            EducationLevel = source.EducationLevel.Name,
        };

    public override Education Map(EducationModel source) =>
        new(
            source.DateStart,
            source.Institution,
            source.EducationLevelId,
            source.Specialty,
            source.DateEnd
        );

    public override void SetProperties(EducationModel source, Education target)
    {
        target.Update(
            source.DateStart,
            source.DateEnd,
            source.Institution,
            source.Specialty,
            source.EducationLevelId
        );
    }
}
