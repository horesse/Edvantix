using Edvantix.Chassis.Converter;
using Edvantix.OrganizationManagement.Features.Member.Models;

namespace Edvantix.OrganizationManagement.Features.Member.Mappers;

public sealed class MemberConverter
    : ClassConverter<MemberModel, Domain.AggregatesModel.MemberAggregate.Member>
{
    public override Domain.AggregatesModel.MemberAggregate.Member Map(MemberModel source)
    {
        return new Domain.AggregatesModel.MemberAggregate.Member(
            source.OrganizationId,
            source.PersonId,
            source.Position
        );
    }

    public override MemberModel Map(Domain.AggregatesModel.MemberAggregate.Member source)
    {
        return new MemberModel
        {
            Id = source.Id,
            OrganizationId = source.OrganizationId,
            PersonId = source.PersonId,
            Position = source.Position,
        };
    }

    public override void SetProperties(
        MemberModel source,
        Domain.AggregatesModel.MemberAggregate.Member target
    )
    {
        target.UpdatePosition(source.Position);
    }
}

