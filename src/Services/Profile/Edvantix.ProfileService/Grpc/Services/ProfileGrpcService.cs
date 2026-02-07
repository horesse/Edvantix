using Edvantix.Chassis.Exceptions;
using Edvantix.ProfileService.Features.ProfileFeature.GetProfileByAccountId;
using Edvantix.ProfileService.Features.ProfileFeature.GetProfileById;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.ProfileService.Grpc.Services;

/// <summary>
/// gRPC сервис для работы с профилями пользователей
/// </summary>
public sealed class ProfileService(ISender sender, ILogger<ProfileService> logger)
    : ProfileGrpcService.ProfileGrpcServiceBase
{
    /// <summary>
    /// Получить профиль по AccountId (GUID пользователя)
    /// </summary>
    [Authorize]
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<ProfileReply> GetProfileByAccountId(
        GetProfileByAccountIdRequest request,
        ServerCallContext context
    )
    {
        try
        {
            if (!Guid.TryParse(request.AccountId, out var accountId))
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Некорректный формат AccountId")
                );
            }

            var query = new GetProfileByAccountIdQuery(accountId);
            var profile = await sender.Send(query, context.CancellationToken);

            return new ProfileReply
            {
                Id = profile.Id,
                AccountId = profile.AccountId.ToString(),
                Gender = (int)profile.Gender,
                BirthDate = profile.BirthDate.ToString("yyyy-MM-dd"),
                FullName = profile.FullName,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                MiddleName = profile.MiddleName ?? string.Empty,
            };
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Профиль не найден для AccountId: {AccountId}",
                request.AccountId
            );
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка при получении профиля по AccountId: {AccountId}",
                request.AccountId
            );
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Получить профиль по внутреннему ID профиля
    /// </summary>
    [Authorize]
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<ProfileReply> GetProfileById(
        GetProfileByIdRequest request,
        ServerCallContext context
    )
    {
        try
        {
            var query = new GetProfileByIdQuery(request.ProfileId);
            var profile = await sender.Send(query, context.CancellationToken);

            return new ProfileReply
            {
                Id = profile.Id,
                AccountId = profile.AccountId.ToString(),
                Gender = (int)profile.Gender,
                BirthDate = profile.BirthDate.ToString("yyyy-MM-dd"),
                FullName = profile.FullName,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                MiddleName = profile.MiddleName ?? string.Empty,
            };
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Профиль не найден для ID: {ProfileId}", request.ProfileId);
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка при получении профиля по ID: {ProfileId}",
                request.ProfileId
            );
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера"));
        }
    }
}
