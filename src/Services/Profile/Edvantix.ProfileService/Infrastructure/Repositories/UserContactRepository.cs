using Edvantix.Chassis.Repository.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;

namespace Edvantix.ProfileService.Infrastructure.Repositories;

public sealed class UserContactRepository(IServiceProvider provider)
    : SoftDeleteRepository<ProfileContext, UserContact, long>(provider),
        IUserContactRepository;
