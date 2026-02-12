using Edvantix.Chassis.Converter;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using MediatR;

namespace Edvantix.ProfileService.Features.UserContactFeature.Features.UpdateOwnContacts;

/// <summary>
/// Команда для обновления контактов текущего пользователя
/// </summary>
public sealed record UpdateOwnContactsCommand(IEnumerable<UserContactModel> Contacts)
    : IRequest<Unit>;

/// <summary>
/// Обработчик команды обновления контактов текущего пользователя
/// </summary>
public sealed class UpdateOwnContactsCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOwnContactsCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateOwnContactsCommand request,
        CancellationToken cancellationToken
    )
    {
        var userGuid = provider.GetUserId();
        using var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountSpecification(userGuid, true);
        var profile = await profileRepo.GetFirstByExpressionAsync(spec, cancellationToken);

        if (profile is null)
            throw new NotFoundException("Профиль не найден.");

        await using var transaction = await profileRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var converter = provider.GetRequiredService<
                IConverter<UserContactModel, UserContact>
            >();
            var contacts = converter.Map(request.Contacts.ToList());
            profile.ReplaceContacts(contacts);

            await profileRepo.UpdateAsync(profile, cancellationToken);
            await profileRepo.SaveEntitiesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
