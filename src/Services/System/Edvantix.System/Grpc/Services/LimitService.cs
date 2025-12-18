using Edvantix.Chassis.Repository.Crud;
using Edvantix.System.Domain.AggregatesModel.LimitAggregate;
using Edvantix.System.Domain.AggregatesModel.LimitAggregate.Specifications;
using Grpc.Core;

namespace Edvantix.System.Grpc.Services;

public sealed class LimitService(IServiceProvider serviceProvider)
    : LimitGrpcService.LimitGrpcServiceBase
{
    private readonly ILogger<LimitService> _logger = serviceProvider.GetRequiredService<
        ILogger<LimitService>
    >();

    public override async Task<DecimalValue> GetLimitValue(
        GetLimitRequest request,
        ServerCallContext context
    )
    {
        var limitType = (LimitType)request.LimitType;
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "[{Service}] - Получение значения лимита {Limit} для подписки {Id}",
                nameof(System),
                limitType,
                request.SubscriptionId
            );
        }

        var spec = new LimitSpecification(request.SubscriptionId, (LimitType)request.LimitType);

        using var repo = serviceProvider.GetRequiredService<ICrudRepository<Limit, long>>();

        var result =
            await repo.GetFirstByExpressionAsync(spec, context.CancellationToken)
            ?? throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"Не найден лимит {limitType} для подписки {request.SubscriptionId}"
                )
            );

        return result.Value!;
    }
}
