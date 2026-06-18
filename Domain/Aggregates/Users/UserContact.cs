using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Users;

public class UserContact : SoftDeletableEntity<Guid>
{
    public Guid OwnerUserId { get; private set; }
    public Guid ContactUserId { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }

    private UserContact() { }

    private UserContact(Guid id, Guid ownerUserId, Guid userId, string firstName, string? lastName) : base(id)
    {
        OwnerUserId = ownerUserId;
        ContactUserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }

    public static UserContact Create(Guid ownerUserId, Guid userId, string firstName, string? lastName)
        => new(Guid.NewGuid(), ownerUserId, userId, firstName, lastName);

    public void Update(string firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}