using Edvantix.Chassis.Repository.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;

namespace Edvantix.ProfileService.Infrastructure.Repositories;

public sealed class ProfileRepository(IServiceProvider provider)
    : CrudRepository<ProfileContext, Domain.AggregatesModel.ProfileAggregate.Profile, long>(
        provider
    ),
        IProfileRepository;
