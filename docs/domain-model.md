# Domain Model

> Audit date: 2026-06-18

## Building blocks

- `Entity<TId>` — identity equality.
- `AuditableEntity` / `SoftDeletableEntity<TId>` — `CreatedAtUtc/By`, `UpdatedAtUtc/By`,
  `IsDeleted` + `MarkAsDelete()`.
- `AggregateRoot<TId>` — holds `DomainEvents`, `RaiseDomainEvent`, `ClearDomainEvents`
  (⚠️ raising currently disabled everywhere).
- `ValueObject` — structural equality base.

## Aggregates

### User (`Aggregates/Users/User.cs`) — aggregate root
- **Owns:** `UserContact`, `BlockedUser`, `UserProfilePhoto`, `UserSession`, and the
  `UserPrivacySettings` value object.
- **Invariants enforced:** cannot add/block self; contacts and blocks are de-duplicated;
  session reuse per `DeviceId` while active; revoke single/all sessions.
- **Behavior:** `Create`, `CompleteProfile`, `UpdateProfile`, `SetUsername`,
  `ChangePhoneNumber`, `AddProfilePhoto`, session management, `UpdatePrivacySettings`,
  contact + block management.
- **Gaps:** `LastSeenAt` commented out; no profile-photo-current pointer logic.

### Chat (`Aggregates/Chats/Chat.cs`) — aggregate root
- **Owns:** `ChatParticipant`, `PinnedMessage`.
- **Types:** `Private`, `SavedMessages`, `Group`, `Channel` (`ChatType`).
- **Invariants:** private chats can't edit info or add members; only `Owner` can edit info;
  participants de-duplicated; owner can't be promoted/demoted.
- **Behavior:** factory per type, `EditInfo`, participant add/remove, promote/demote,
  `MarkAsRead`, `PinMessage`, per-user mute/archive/pin.
- **Gaps:** no owner-leave ownership transfer rule wired; channel posting permissions absent;
  no public username uniqueness rule in-domain (enforced only by DB index).

### Message (`Aggregates/Messages/Message.cs`) — aggregate root
- **Owns:** `MessageAttachment`, `MessageReaction`.
- **Types:** `MessageType` (Text and others).
- **Invariants:** edit requires non-empty content; reactions unique per (user, emoji);
  delete clears content + soft-deletes.
- **Behavior:** `Send`, `SendReply`, `Forward`, `Edit`, `Delete`, `AddAttachment`,
  `AddReaction`, `RemoveReaction`.
- **Gaps:** events not raised; no edit-time-window rule; forward author-hiding not enforced.

### OtpCode (`Aggregates/Auth/OtpCode.cs`) — aggregate root
- One-time codes by phone + `OtpPurpose`; hashed code storage.

### File (`Aggregates/Files/File.cs`) — aggregate root
- Media metadata: `FileName`, `StorageKey`, `ContentType`, `Type`, `UploadedBy`.
- **Gaps:** no storage integration writes/reads these yet.

## Value Objects (`Domain/ValueObjects`)
`PhoneNumber`, `Username`, `Email`, `ChatTitle`, `MessageContent`, `FilePath`,
plus `UserPrivacySettings` (under Users).

## Enums (`Domain/Enums`)
`ChatRole`, `ChatType`, `FileType`, `MessageType`, `NotificationType`, `OtpPurpose`,
`PrivacyLevel`.

## Domain Events (`Domain/Events`) — defined but NOT raised
- Chats: `ChatCreatedEvent`, `ParticipantAddedEvent`, `ParticipantRemovedEvent`.
- Messages: `MessageSentEvent`, `MessageEditedEvent`, `MessageDeletedEvent`,
  `MessageReactionAddedEvent`.

> ⚠️ All call sites are commented out and there is no dispatcher. Re-enabling these and adding
> dispatch (see `architecture.md` A1) is a prerequisite for realtime, notifications, and read
> models.

## Repositories (interfaces in Application, impl in Persistence)
`IUserRepository`, `IChatRepository`, `IMessageRepository`, `IOtpCodeRepository`,
`IFileRepository` (no impl yet), plus `IUnitOfWork`.

## Aggregate relationships (logical)

```
User 1───* UserSession
User 1───* UserContact ───> User (contactUserId)
User 1───* BlockedUser ───> User (blockedUserId)
User 1───* UserProfilePhoto ───> File
Chat 1───* ChatParticipant ───> User
Chat 1───* PinnedMessage  ───> Message
Chat 1───* Message
Message 1───* MessageAttachment ───> File
Message 1───* MessageReaction ───> User
Message 0..1 ──> Message (ReplyToMessageId)
Message 0..1 ──> Message (ForwardedFromMessageId)
```
