using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Users;

public class BlockedUser : SoftDeletableEntity<Guid>
{
    public Guid OwnerUserId { get; private set; }
    public Guid BlockedUserId { get; private set; }

    private BlockedUser() { }

    private BlockedUser(Guid id, Guid ownerUserId, Guid blockedUserId) : base(id)
    {
        BlockedUserId = blockedUserId;
        OwnerUserId = ownerUserId;
    }

    public static BlockedUser Create(Guid ownerUserId, Guid blockedUserId)
        => new(Guid.NewGuid(), ownerUserId, blockedUserId);
}