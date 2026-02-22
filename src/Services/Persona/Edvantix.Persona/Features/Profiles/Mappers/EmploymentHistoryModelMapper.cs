namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>Maps <see cref="EmploymentHistory"/> to <see cref="EmploymentHistoryModel"/>.</summary>
public sealed class EmploymentHistoryModelMapper : Mapper<EmploymentHistory, EmploymentHistoryModel>
{
    public override EmploymentHistoryModel Map(EmploymentHistory source) =>
        new(
            source.Workplace,
            source.Position,
            source.StartDate,
            source.EndDate,
            source.Description
        );
}
