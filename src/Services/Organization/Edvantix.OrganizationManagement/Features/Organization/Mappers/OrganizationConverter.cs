using Edvantix.Chassis.Converter;
using Edvantix.OrganizationManagement.Features.Organization.Models;

namespace Edvantix.OrganizationManagement.Features.Organization.Mappers;

public sealed class OrganizationConverter
    : ClassConverter<OrganizationModel, Domain.AggregatesModel.OrganizationAggregate.Organization>
{
    public override Domain.AggregatesModel.OrganizationAggregate.Organization Map(
        OrganizationModel source
    )
    {
        return new Domain.AggregatesModel.OrganizationAggregate.Organization(
            source.Name,
            source.NameLatin,
            source.ShortName,
            source.RegistrationDate,
            source.PrintName,
            source.Description
        );
    }

    public override OrganizationModel Map(
        Domain.AggregatesModel.OrganizationAggregate.Organization source
    )
    {
        return new OrganizationModel
        {
            Id = source.Id,
            Name = source.Name,
            NameLatin = source.NameLatin,
            ShortName = source.ShortName,
            PrintName = source.PrintName,
            Description = source.Description,
            RegistrationDate = source.RegistrationDate,
        };
    }

    public override void SetProperties(
        OrganizationModel source,
        Domain.AggregatesModel.OrganizationAggregate.Organization target
    )
    {
        // Read-only, no update implementation needed
        throw new NotSupportedException("Organization updates are not supported via API");
    }
}

