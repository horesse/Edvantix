using Edvantix.Chassis.Repository.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

namespace Edvantix.ProfileService.Infrastructure.Repositories;

public sealed class EducationRepository(IServiceProvider provider)
    : SoftDeleteRepository<ProfileContext, Education, long>(provider),
        IEducationRepository;
