namespace Edvantix.Persona.Features.EmploymentHistories;

public sealed class DomainToDtoMapper : Mapper<EmploymentHistory, EmploymentHistoryDto>
{
    public override EmploymentHistoryDto Map(EmploymentHistory source) =>
        new(
            source.Workplace,
            source.Position,
            source.StartDate,
            source.EndDate,
            source.Description
        );
}
