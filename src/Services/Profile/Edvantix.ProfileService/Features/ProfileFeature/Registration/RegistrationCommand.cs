using System.Security.Claims;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Infrastructure.Blob;
using MediatR;

namespace Edvantix.ProfileService.Features.ProfileFeature.Registration;

public sealed record RegistrationCommand(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate,
    Gender Gender,
    IFormFile? Avatar
) : IRequest<long>;

public sealed class RegistrationCommandHandler(IServiceProvider provider)
    : IRequestHandler<RegistrationCommand, long>
{
    public async Task<long> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var userGuid = provider.GetUserId();

        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var isExists = await profileRepo.AnyAsync(p => p.AccountId == userGuid, cancellationToken);

        if (isExists)
            throw new Exception("Пользователь с таким идентификатором уже существует");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var profile = new Profile(
                userGuid,
                request.Gender,
                request.BirthDate,
                request.FirstName,
                request.LastName,
                request.MiddleName
            );

            if (request.Avatar is not null)
            {
                var blobService = provider.GetRequiredService<IBlobService>();

                string? url = request.Avatar is null
                    ? null
                    : await blobService.UploadFileAsync(request.Avatar, cancellationToken);

                profile.UploadAvatar(url);
            }

            await profileRepo.InsertAsync(profile, cancellationToken);

            await profileRepo.SaveEntitiesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return profile.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
