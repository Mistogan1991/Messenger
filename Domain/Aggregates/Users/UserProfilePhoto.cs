using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Users;

public class UserProfilePhoto : SoftDeletableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid FileId { get; private set; }

    private UserProfilePhoto() { }

    private UserProfilePhoto(Guid id, Guid userId, Guid fileId) : base(id)
    {
        UserId = userId;
        FileId = fileId;
    }

    public static UserProfilePhoto Create(Guid userId, Guid fileId)
        => new(Guid.NewGuid(), userId, fileId);
}