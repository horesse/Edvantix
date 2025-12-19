using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.UsageAggregate;
using Edvantix.Company.Domain.AggregatesModel.UsageAggregate.Specifications;
using Edvantix.OrganizationManagement.Grpc.Services;
using Grpc.Core;

namespace Edvantix.Company.Grpc.Services;

public sealed class UsageService(IServiceProvider serviceProvider)
    : UsageGrpcService.UsageGrpcServiceBase
{
    private readonly ILogger<UsageService> _logger = serviceProvider.GetRequiredService<
        ILogger<UsageService>
    >();

    public override async Task<UsageResponse> UpdateUsageValue(
        UpdateUsageRequest request,
        ServerCallContext context
    )
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "[{Service}] - Обновление значения использования для организации {OrganizationId} и лимита {LimitId}",
                nameof(OrganizationManagement),
                request.OrganizationId,
                request.LimitId
            );
        }

        var spec = new UsageSpecification
        {
            OrganizationId = request.OrganizationId,
            LimitId = request.LimitId,
        };

        using var repo = serviceProvider.GetRequiredService<ICrudRepository<Usage, long>>();

        var usage = await repo.GetFirstByExpressionAsync(spec, context.CancellationToken);

        if (usage == null)
        {
            // Create new usage record
            usage = new Usage(request.OrganizationId, request.LimitId, (decimal)request.Value);
            await repo.InsertAsync(usage, context.CancellationToken);
        }
        else
        {
            // Update existing usage
            usage.UpdateValue((decimal)request.Value);
            await repo.UpdateAsync(usage, context.CancellationToken);
        }

        await repo.SaveEntitiesAsync(context.CancellationToken);

        return new UsageResponse
        {
            Id = usage.Id,
            OrganizationId = usage.OrganizationId,
            LimitId = usage.LimitId,
            Value = (double)usage.Value,
        };
    }
}
