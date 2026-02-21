namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>Maps <see cref="Education"/> to <see cref="EducationModel"/>.</summary>
public sealed class EducationModelMapper : Mapper<Education, EducationModel>
{
    public override EducationModel Map(Education source) =>
        new(
            source.DateStart,
            source.DateEnd,
            source.Institution,
            source.Specialty,
            source.EducationLevel
        );
}
