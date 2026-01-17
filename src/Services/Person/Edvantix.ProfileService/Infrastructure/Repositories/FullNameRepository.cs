using Edvantix.Chassis.Repository.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;

namespace Edvantix.ProfileService.Infrastructure.Repositories;

public sealed class FullNameRepository(IServiceProvider provider)
    : SoftDeleteRepository<ProfileContext, FullName, long>(provider),
        IFullNameRepository;
