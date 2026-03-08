using Edvantix.Persona.Grpc.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Persona.Grpc.Services;

/// <summary>
/// gRPC-сервис профилей для межсервисного взаимодействия.
/// Обращается к репозиторию напрямую, минуя HTTP-маппер (SAS URL аватара не генерируется).
/// </summary>
[AllowAnonymous]
public sealed class ProfileService(IProfileRepository profileRepo, ILogger<ProfileService> logger)
    : ProfileGrpcService.ProfileGrpcServiceBase
{
    /// <summary>
    /// Возвращает краткий профиль по ProfileId или AccountId (Keycloak GUID).
    /// Ровно одно поле из oneof должно быть заполнено.
    /// </summary>
    public override async Task<ProfileReply> GetProfile(
        GetProfileRequest request,
        ServerCallContext context
    )
    {
        try
        {


            ISpecification<Profile> spec = new ProfileByIdSpec(Guid.Parse(request.ProfileId));

            var profile = await profileRepo.FindAsync(spec, context.CancellationToken);

            if (profile is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Профиль не найден."));

            return new ProfileReply
            {
                Id = profile.Id.ToString(),
                AccountId = profile.AccountId.ToString(),
                Gender = (int)profile.Gender,
                BirthDate = profile.BirthDate.ToString("yyyy-MM-dd"),
                FullName = profile.FullName.GetFullName(),
                FirstName = profile.FullName.FirstName,
                LastName = profile.FullName.LastName,
                MiddleName = profile.FullName.MiddleName ?? string.Empty,
                Login = profile.Login,
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "gRPC GetProfile: внутренняя ошибка. Request: {Request}", request);
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера."));
        }
    }
}
