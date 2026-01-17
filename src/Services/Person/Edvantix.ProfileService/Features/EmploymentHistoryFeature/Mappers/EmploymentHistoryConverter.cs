using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature.Mappers;

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
            CompanyName = source.Workplace,
            Position = source.Position,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Description = source.Description,
            Id = source.Id,
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
