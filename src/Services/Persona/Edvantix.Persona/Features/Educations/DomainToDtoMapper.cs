namespace Edvantix.Persona.Features.Educations;

public sealed class DomainToDtoMapper : Mapper<Education, EducationDto>
{
    public override EducationDto Map(Education source) =>
        new(
            source.DateStart,
            source.DateEnd,
            source.Institution,
            source.Specialty,
            source.EducationLevel
        );
}
