namespace Messenger.Domain.Common;

public abstract class SoftDeletableEntity<TId> : AuditableEntity<TId> where TId : notnull
{
    public DateTime? DeletedAtUtc { get; protected set; }

    public bool IsDeleted => DeletedAtUtc.HasValue;

    protected SoftDeletableEntity() { }

    protected SoftDeletableEntity(TId id) : base(id) { }

    public void MarkAsDelete()
    {
        DeletedAtUtc = DateTime.UtcNow;
    }
}