using Messenger.Domain.Common;
using Messenger.Domain.Common.Exceptions;
using Messenger.Domain.Enums;
using Messenger.Domain.ValueObjects;

namespace Messenger.Domain.Aggregates.Users;

public class User : AggregateRoot<Guid>
{
    #region Fields

    private readonly List<UserContact> _contacts = [];
    private readonly List<BlockedUser> _blockedUsers = [];
    private readonly List<UserProfilePhoto> _photos = [];
    private readonly List<UserSession> _sessions = [];

    #endregion

    #region Properties

    public PhoneNumber PhoneNumber { get; private set; }
    public Username? Username { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Bio { get; private set; }
    public UserPrivacySettings PrivacySettings { get; private set; }
    //public DateTime LastSeenAt { get; private set; }

    public IReadOnlyCollection<UserContact> Contacts => _contacts;
    public IReadOnlyCollection<BlockedUser> BlockedUsers => _blockedUsers;
    public IReadOnlyCollection<UserProfilePhoto> Photos => _photos;
    public IReadOnlyCollection<UserSession> Sessions => _sessions;

    #endregion

    #region Constructors

    private User() { }

    private User(Guid id, PhoneNumber phone) : base(id)
    {
        PhoneNumber = phone;
        PrivacySettings = UserPrivacySettings.Default();
        //LastSeenAt = DateTime.UtcNow;
    }

    #endregion

    #region Factory Methods

    public static User Create(PhoneNumber phone)
    {
        return new User(Guid.NewGuid(), phone);
    }

    #endregion

    #region Domain Behavior

    /* ====================== */
    /* ==== User Profile ==== */
    /* ====================== */

    public void CompleteProfile(string firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;

        SetUpdated(Id);
    }

    public void UpdateProfile(string firstName, string? lastName, string? bio)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        SetUpdated(Id);
    }

    public void SetUsername(Username username)
    {
        if (Username == username) return;

        Username = username;
        SetUpdated(Id);
    }

    public void ChangePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
        SetUpdated(Id);
    }

    public void AddProfilePhoto(Guid fileId)
    {
        _photos.Add(UserProfilePhoto.Create(Id, fileId));
    }

    //public void SetLastSeen()
    //{
    //    LastSeenAt = DateTime.UtcNow;
    //    SetUpdated(Id);
    //}

    /* ====================== */
    /* ==== User Session ==== */
    /* ====================== */

    public UserSession AddSession(
        string deviceId,
        string deviceName,
        string deviceType,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAtUtc)
    {
        var existingSession = _sessions
            .FirstOrDefault(x => x.DeviceId == deviceId && x.RevokedAtUtc == null);

        if (existingSession is not null)
        {
            existingSession.UpdateActivity();

            return existingSession;
        }

        var session = UserSession.Create(
            Id,
            deviceId,
            deviceName,
            deviceType,
            refreshTokenHash,
            refreshTokenExpiresAtUtc);

        _sessions.Add(session);

        return session;
    }

    public UserSession? GetActiveSession(string deviceId)
    {
        return _sessions
            .FirstOrDefault(x =>
                x.DeviceId == deviceId &&
                x.IsActive());
    }

    public UserSession? GetSession(Guid sessionId)
    {
        return _sessions
            .FirstOrDefault(x => x.Id == sessionId);
    }

    public void RevokeSession(Guid sessionId)
    {
        var session = _sessions.SingleOrDefault(x => x.Id == sessionId);

        if (session is null)
            throw new DomainException("Session not found.");

        session.Revoke();
    }

    public void RevokeAllSessions(Guid exceptSessionId)
    {
        var activeSessions = _sessions.Where(x =>
                        x.Id != exceptSessionId &&
                        x.RevokedAtUtc is null);


        foreach (var session in activeSessions)
        {
            session.Revoke();
        }
    }

    /* ====================== */
    /* == Privacy Settings == */
    /* ====================== */

    public void UpdatePrivacySettings(
        PrivacyLevel lastSeen,
        PrivacyLevel phoneNumber,
        PrivacyLevel profilePhoto,
        PrivacyLevel calls,
        PrivacyLevel forwardedMessages)
    {
        PrivacySettings = PrivacySettings.Update(
            lastSeen,
            phoneNumber,
            profilePhoto,
            calls,
            forwardedMessages);

        SetUpdated(Id);
    }

    /* ===================== */
    /* ====== Contact ====== */
    /* ===================== */

    public void AddContact(Guid contactUserId, string firstName, string? lastName)
    {
        if (contactUserId == Id)
            throw new DomainException("User cannot add himself.");

        if (_contacts.Any(x => x.ContactUserId == contactUserId)) return;

        _contacts.Add(UserContact.Create(Id, contactUserId, firstName, lastName));
    }

    public void UpdateContact(Guid contactUserId, string firstName, string? lastName)
    {
        var contact = _contacts
            .FirstOrDefault(x => x.ContactUserId == contactUserId);

        if (contact is null)
            throw new DomainException("Contact not found.");

        contact.Update(firstName, lastName);
    }

    public void DeleteContact(Guid contactUserId)
    {
        var contact = _contacts
            .FirstOrDefault(x => x.ContactUserId == contactUserId);

        if (contact is null) return;

        _contacts.Remove(contact);
    }

    /* ===================== */
    /* === Blocked Users === */
    /* ===================== */

    public void BlockUser(Guid userId)
    {
        if (userId == Id)
            throw new DomainException("User cannot block himself.");

        if (_blockedUsers.Any(x => x.BlockedUserId == userId)) return;

        _blockedUsers.Add(BlockedUser.Create(Id, userId));
    }

    public void UnblockUser(Guid userId)
    {
        var blocked = _blockedUsers
            .FirstOrDefault(x => x.BlockedUserId == userId);

        if (blocked is null) return;

        _blockedUsers.Remove(blocked);
    }

    #endregion
}