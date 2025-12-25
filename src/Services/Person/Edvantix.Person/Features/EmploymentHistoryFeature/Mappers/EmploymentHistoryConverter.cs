using Edvantix.Chassis.Converter;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Features.EmploymentHistoryFeature.Models;

namespace Edvantix.Person.Features.EmploymentHistoryFeature.Mappers;

public class EmploymentHistoryConverter : ClassConverter<EmploymentHistoryModel, EmploymentHistory>
{
    public override EmploymentHistory Map(EmploymentHistoryModel source) =>
        new(
            source.CompanyName,
            source.Position,
            source.StartDate,
            source.EndDate,
            source.Description
        );

    public override EmploymentHistoryModel Map(EmploymentHistory source) =>
        new()
        {
            CompanyName = source.CompanyName,
            Position = source.Position,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Description = source.Description,
            Id = source.Id
        };

    public override void SetProperties(EmploymentHistoryModel source, EmploymentHistory target)
    {
        target.Update(
            source.CompanyName,
            source.Position,
            source.StartDate,
            source.EndDate,
            source.Description
        );
    }
}
