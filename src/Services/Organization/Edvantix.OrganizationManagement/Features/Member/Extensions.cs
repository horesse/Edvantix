using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate.Specifications;
using Edvantix.OrganizationManagement.Features.Member.Models;

namespace Edvantix.OrganizationManagement.Features.Member;

public static class Extensions
{
    public static IServiceCollection AddMemberFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<
            MemberModel,
            Guid,
            Domain.AggregatesModel.MemberAggregate.Member,
            MemberSpecification
        >();

        services.AddCrudEndpoints<
            Domain.AggregatesModel.MemberAggregate.Member,
            MemberModel,
            Guid,
            MemberSpecification
        >();

        return services;
    }
}

