namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>Репозиторий агрегата <see cref="Invitation"/>.</summary>
public interface IInvitationRepository : IRepository<Invitation>
{
    /// <summary>Возвращает приглашение по идентификатору.</summary>
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает активное (Pending) приглашение по хэшу токена.</summary>
    Task<Invitation?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает приглашения по спецификации.</summary>
    Task<IReadOnlyCollection<Invitation>> ListAsync(
        ISpecification<Invitation> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает приглашения по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<Invitation> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет приглашение.</summary>
    Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default);
}
