using Messenger.Domain.Common;
using Messenger.Domain.Common.Exceptions;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Chats;

public sealed class Chat : AggregateRoot<Guid>
{
    public ChatType Type { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public Guid? PhotoId { get; private set; }
    public bool? IsPublic { get; private set; }
    public string? Username { get; private set; }

    private readonly List<ChatParticipant> _participants = [];
    private readonly List<PinnedMessage> _pinnedMessages = [];

    public IReadOnlyCollection<ChatParticipant> Participants => _participants;
    public IReadOnlyCollection<PinnedMessage> PinnedMessages => _pinnedMessages;

    private Chat() { }

    private Chat(Guid id, ChatType type) : base(id)
    {
        Type = type;
    }

    #region Factory Methods

    public static Chat CreatePrivate(Guid user1, Guid user2)
    {
        var chat = new Chat(Guid.NewGuid(), ChatType.Private);

        chat._participants.Add(ChatParticipant.Create(chat.Id, user1, ChatRole.Member));

        chat._participants.Add(ChatParticipant.Create(chat.Id, user2, ChatRole.Member));

        return chat;
    }

    public static Chat CreateSavedMessages(Guid userId)
    {
        var chat = new Chat(Guid.NewGuid(), ChatType.SavedMessages);

        chat._participants.Add(ChatParticipant.Create(chat.Id, userId, ChatRole.Owner));

        return chat;
    }
    public static Chat CreateGroup(string title, Guid ownerId, IEnumerable<Guid> memberIds)
    {
        var chat = new Chat(Guid.NewGuid(), ChatType.Group);

        chat.Title = title;

        chat._participants.Add(ChatParticipant.Create(chat.Id, ownerId, ChatRole.Owner));

        chat.AddParticipants(memberIds.Where(id => id != ownerId));

        return chat;
    }
    public static Chat CreateGroup(string title, Guid ownerId, bool isPublic)
    {
        var chat = new Chat(Guid.NewGuid(), ChatType.Group);

        chat.Title = title;
        chat.IsPublic = isPublic;

        chat._participants.Add(ChatParticipant.Create(chat.Id, ownerId, ChatRole.Owner));

        return chat;
    }

    public static Chat CreateChannel(string title, Guid ownerId, bool isPublic)
    {
        var chat = new Chat(Guid.NewGuid(), ChatType.Channel);

        chat.Title = title;
        chat.IsPublic = isPublic;

        chat._participants.Add(ChatParticipant.Create(chat.Id, ownerId, ChatRole.Owner));

        return chat;
    }

    #endregion

    #region Domain Rules
    //ChangePhoto(...)
    //ChangeUsername(...)

    public void EditInfo(Guid userId, string title, string? description, bool? isPublic = false)
    {
        if (Type == ChatType.Private)
            throw new DomainException("Private chat cannot edit info.");

        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant!.Role != ChatRole.Owner)
            throw new DomainException("You cannot edit info.");

        Title = title;
        Description = description;
        IsPublic = isPublic;
    }

    public ChatParticipant GetParticipant(Guid userId)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant is null)
            throw new DomainException("Participant not found.");

        return participant;
    }

    public void AddParticipant(Guid userId)
    {
        if (Type == ChatType.Private)
            throw new DomainException("Private chat cannot add members.");

        if (_participants.Any(x => x.UserId == userId)) return;

        _participants.Add(ChatParticipant.Create(Id, userId, ChatRole.Member));
    }

    public void RemoveParticipant(Guid userId)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant is null) return;

        _participants.Remove(participant);
    }

    public void AddParticipants(IEnumerable<Guid> userIds)
    {
        foreach (var userId in userIds)
        {
            AddParticipant(userId);
        }
    }

    public void MarkAsRead(Guid userId, Guid messageId)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant is null)
            throw new DomainException("Member not found");

        participant.MarkAsRead(messageId);
    }

    public void PinMessage(Guid messageId, Guid userId)
    {
        if (_pinnedMessages.Any(x => x.MessageId == messageId)) return;

        _pinnedMessages.Add(PinnedMessage.Create(Id, messageId, userId));
    }

    public void PromoteAdmin(Guid userId)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant is null)
            throw new DomainException("Member not found.");

        participant.PromoteToAdmin();
    }

    public void DemoteAdmin(Guid userId)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant is null)
            throw new DomainException("Member not found");

        participant.DemoteAdmin();
    }

    public void MuteChat(Guid userId)
    {
        GetParticipant(userId).Mute();
    }

    public void UnMuteChat(Guid userId)
    {
        GetParticipant(userId).UnMute();
    }

    public void ArchiveChat(Guid userId)
    {
        GetParticipant(userId).Archive();
    }

    public void UnArchiveChat(Guid userId)
    {
        GetParticipant(userId).UnArchive();
    }

    public void PinChat(Guid userId)
    {
        GetParticipant(userId).Pin();
    }

    public void UnPinChat(Guid userId)
    {
        GetParticipant(userId).UnPin();
    }
    #endregion
}