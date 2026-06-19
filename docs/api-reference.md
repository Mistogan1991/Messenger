# API Reference

> Audit date: 2026-06-18 · All routes prefixed `/api`. All except auth `request/verify-otp`
> require a Bearer JWT. Responses wrap the `Result<T>` envelope.

## Auth — `/api/auth`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| POST | `request-otp` | `RequestOtpCommand` | ✅ |
| POST | `verify-otp` | `VerifyOtpCommand` | ✅ |
| POST | `refresh-token` | `RefreshTokenCommand` | ✅ |
| POST | `logout` | `LogoutCommand` | ✅ |
| POST | `revoke-session` | `RevokeSessionCommand` | ✅ |
| GET | `sessions` | `GetSessionsQuery` | ✅ |

## Profile — `/api/profile`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| PUT | `request-phone-number-change-otp` | `RequestPhoneNumberChangeOtpCommand` | ✅ |
| PUT | `confirm-phone-number-change` | `ConfirmPhoneNumberChangeCommand` | ✅ |
| PUT | `username` | `SetUsernameCommand` | ✅ |
| GET | `` | `GetMyProfileQuery` | ✅ |
| PUT | `` | `UpdateMyProfileCommand` | ✅ |
| GET | `privacy` | `GetPrivacySettingsQuery` | ✅ |
| PUT | `privacy` | `UpdatePrivacySettingsCommand` | ✅ |

## Users — `/api/user`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| GET | `user-profile/{id}` | `GetUserProfileQuery` | ✅ |

## Contacts — `/api/contacts`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| POST | `` | `AddContactCommand` | ✅ |
| PUT | `` | `UpdateContactCommand` | ✅ |
| DELETE | `{contactUserId}` | `DeleteContactCommand` | ✅ |
| GET | `` | `GetContactsQuery` | ✅ |
| GET | `search` | `SearchContactsQuery` | ✅ |

## Blocked users — `/api/blocked-users`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| POST | `{userId}` | `BlockUserCommand` | ✅ |
| DELETE | `{userId}` | `UnblockUserCommand` | ✅ |
| GET | `` | `GetBlockedUsersQuery` | ✅ |

## Chats — `/api/chats`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| POST | `groups` | `CreateGroupCommand` | ✅ |
| POST | `channels` | `CreateChannelCommand` | ✅ |
| POST | `private/{userId}` | `StartPrivateChatCommand` | ✅ _(M1)_ idempotent |
| POST | `{chatId}/join` | `JoinChannelCommand` | ✅ |
| POST | `{chatId}/leave` | `LeaveChatCommand` | ✅ |
| POST | `{chatId}/members` | `AddMembersCommand` | ✅ |
| DELETE | `{chatId}/members/{userId}` | `RemoveMemberCommand` | ✅ |
| POST | `{chatId}/promote-admin/{userId}` | `PromoteAdminCommand` | ✅ |
| POST | `{chatId}/demote-admin/{userId}` | `DemoteAdminCommand` | ✅ |
| GET | `my` | `GetMyChatsQuery` | ✅ _(M1)_ chat list w/ last-message preview + per-user flags |
| GET | `{chatId}` | `GetChatInfoQuery` | ✅ |
| PUT | `{chatId}` | `EditChatCommand` | ✅ |
| POST | `{chatId}/mute` `/unmute` | `MuteChatCommand` / `UnMuteChatCommand` | ✅ |
| POST | `{chatId}/archive` `/unarchive` | `ArchiveChatCommand` / `UnArchiveChatCommand` | ✅ |
| POST | `{chatId}/pin` `/unpin` | `PinChatCommand` / `UnPinChatCommand` | ✅ |
| GET | `{chatId}/members` | `GetChatMembersQuery` | ✅ _(M1)_ member-only; names joined from Users (username left null) |

## Messages — `/api/messages`
| Method | Route | Command/Query | Status |
|---|---|---|---|
| POST | `` | `SendMessageCommand` | 🟡 (membership guard bug) |
| PUT | `{messageId}` | `EditMessageCommand` | ✅ |
| DELETE | `{messageId}` | `DeleteMessageCommand` | ✅ |
| POST | `reply` | `ReplyMessageCommand` | ✅ |
| POST | `forward` | `ForwardMessageCommand` | 🟡 |
| POST | `read` | `ReadMessageCommand` | ✅ |
| POST | `reaction` | `AddReactionCommand` | ✅ |
| DELETE | `reaction` | `RemoveReactionCommand` | ✅ |
| POST | `attachment` | `AddAttachmentCommand` | 🟡 (no upload pipeline) |
| GET | `chat/{chatId}?before=&limit=` | `GetChatMessagesQuery` | ✅ _(M1)_ member-only; keyset paged, newest first, limit≤100 |
| GET | `{messageId}` | `GetMessageQuery` | ✅ _(M1)_ member-only |

> `GetChatMessages` and `GetMessage` were empty template stubs and were implemented in M1
> (not merely wired). Remaining read gaps: none outstanding for chats/messages.
