using Edvantix.Chassis.Repository.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;

namespace Edvantix.ProfileService.Infrastructure.Repositories;

public sealed class EmploymentHistoryRepository(IServiceProvider provider)
    : SoftDeleteRepository<ProfileContext, EmploymentHistory, long>(provider),
        IEmploymentHistoryRepository;
