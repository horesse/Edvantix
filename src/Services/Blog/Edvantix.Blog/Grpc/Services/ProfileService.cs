using System.Diagnostics.CodeAnalysis;
using Edvantix.Chassis.Exceptions;
using Edvantix.ProfileService.Grpc.Services;

namespace Edvantix.Blog.Grpc.Services;

/// <summary>
/// Реализация клиента Profile gRPC-сервиса.
/// Предоставляет данные об авторах постов из микросервиса Profile.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient client)
    : IProfileService
{
    /// <inheritdoc />
    public async Task<long> GetProfileIdByAccountId(
        Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileByAccountIdRequest { AccountId = accountId.ToString() };

        var result = await client.GetProfileByAccountIdAsync(
            request,
            cancellationToken: cancellationToken
        );

        return result?.Id ?? throw new NotFoundException("Профиль пользователя не найден.");
    }

    /// <inheritdoc />
    public async Task<AuthorInfo?> GetAuthorById(
        long profileId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileByIdRequest { ProfileId = profileId };

        var result = await client.GetProfileByIdAsync(
            request,
            cancellationToken: cancellationToken
        );

        if (result is null)
            return null;

        return new AuthorInfo(result.Id, result.FullName, result.FirstName, result.LastName);
    }
}
