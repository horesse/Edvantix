using Edvantix.SharedKernel.Helpers;

namespace Edvantix.SharedKernel.SeedWork;

public abstract class AuditableEntity : LongIdentity
{
    public DateTime CreatedAt { get; init; } = DateTimeHelper.UtcNow();
    public DateTime? LastModifiedAt { get; set; }
    public uint RowVersion { get; set; }
}

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : struct
{
    public DateTime CreatedAt { get; init; } = DateTimeHelper.UtcNow();
    public DateTime? LastModifiedAt { get; set; }
    public uint RowVersion { get; set; }
}
