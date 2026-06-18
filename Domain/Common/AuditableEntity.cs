namespace Messenger.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedAtUtc { get; protected set; }

    public Guid? CreatedBy { get; protected set; }

    public DateTime? UpdatedAtUtc { get; protected set; }

    public Guid? UpdatedBy { get; protected set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(TId id) : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetUpdated(Guid? updatedBy)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}