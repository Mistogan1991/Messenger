using Messenger.Domain.Common;
using Messenger.Domain.Common.Exceptions;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Chats;

public sealed class ChatParticipant : SoftDeletableEntity<Guid>
{
    #region Properties

    public Guid ChatId { get; private set; }
    public Guid UserId { get; private set; }
    public ChatRole Role { get; private set; }
    public bool IsMuted { get; private set; }
    public bool IsArchived { get; private set; }
    public bool IsPinned { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }
    public Guid? LastReadMessageId { get; private set; }

    #endregion

    #region Constructors

    private ChatParticipant() { }

    private ChatParticipant(Guid id, Guid chatId, Guid userId, ChatRole role) : base(id)
    {
        ChatId = chatId;
        UserId = userId;
        Role = role;
        JoinedAtUtc = DateTime.UtcNow;
    }

    #endregion

    #region Factory Methods
    
    public static ChatParticipant Create(Guid chatId, Guid userId, ChatRole role)
    {
        return new ChatParticipant(Guid.NewGuid(), chatId, userId, role);
    }

    #endregion


    #region Domain Rules

    public void MarkAsRead(Guid messageId)
    {
        LastReadMessageId = messageId;
        SetUpdated(UserId);
    }

    public void PromoteToAdmin()
    {
        if (Role == ChatRole.Owner)
            throw new DomainException("Owner cannot be promote to admin.");

        Role = ChatRole.Admin;
        SetUpdated(UserId);
    }

    public void DemoteAdmin()
    {
        if (Role == ChatRole.Owner)
            throw new DomainException("Owner cannot be demoted.");

        Role = ChatRole.Member;
        SetUpdated(UserId);
    }

    public void TransferOwnership()
    {
        Role = ChatRole.Owner;
        SetUpdated(UserId);
    }

    public void Mute()
    {
        IsMuted = true;
    }

    public void UnMute()
    {
        IsMuted = false;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void UnArchive()
    {
        IsArchived = false;
    }
    
    public void Pin()
    {
        IsPinned = true;
    }

    public void UnPin()
    {
        IsPinned = false;
    }

    #endregion
}
