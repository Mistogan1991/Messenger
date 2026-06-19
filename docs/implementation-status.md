# Implementation Status

> Audit date: 2026-06-18 · Branch: `claud` · Build: **succeeds** (0 errors, 13 nullable warnings)

This document is the single source of truth for "what exists today" in the Messenger
backend. It is generated from an audit of the actual code, not from intent.

## 1. Solution structure

Clean Architecture, 6 projects (`Messenger.slnx`):

| Project | Layer | Responsibility |
|---|---|---|
| `Messenger.Domain` | Domain | Aggregates, entities, value objects, domain events, enums |
| `Messenger.Application` | Application | CQRS commands/queries, handlers, DTOs, abstractions |
| `Messenger.Contracts` | Contracts | Shared contracts (currently **empty**) |
| `Messenger.Persistence` | Infrastructure | EF Core DbContext, configurations, repositories, migrations |
| `Messenger.Infrastructure` | Infrastructure | Auth (JWT/OTP/hash), `CurrentUser` |
| `Messenger.API` | Presentation | Controllers, request contracts, mappings, middleware |

Dependency direction is correct: Domain has no outward references; Application depends only
on Domain + Contracts; Infrastructure/Persistence depend inward; API composes everything.

## 2. Feature status matrix

Legend: ✅ Completed · 🟡 Partial · ❌ Missing / scaffolded only

| Feature | Status | Notes |
|---|---|---|
| Auth — OTP request/verify | ✅ | OTP issued, hashed, verified; JWT + refresh token issued |
| Auth — refresh token / sessions | ✅ | Sessions per device, revoke, revoke-all, list |
| User profile | ✅ | Get/update profile, set username, bio |
| Phone number change | ✅ | OTP-gated change flow |
| Privacy settings | ✅ | Get/update, value object `UserPrivacySettings` |
| Contacts | ✅ | Add/update/delete/list/search |
| Blocked users | ✅ | Block/unblock/list |
| Chats — groups | ✅ | Create, edit, members, admins, leave |
| Chats — channels | 🟡 | Create/join exist; no posting rules, no subscriber model |
| Chats — private/saved | 🟡 | `StartPrivateChat` command + endpoint added (idempotent) _(M1)_; Saved Messages still has no command |
| Chat membership ops | ✅ | Add/remove members, promote/demote admin |
| Chat per-user state | ✅ | Mute/archive/pin (on `ChatParticipant`) |
| Messaging — send/edit/delete | ✅ | Text only |
| Messaging — reply | ✅ | `ReplyToMessageId` |
| Messaging — forward | 🟡 | Aggregate + handler exist; forward author-hiding not exercised |
| Reactions | ✅ | Add/remove, unique per (message,user,emoji) |
| Read receipts | 🟡 | `MarkAsRead` sets last-read id; no per-message delivery/seen fan-out |
| Attachments | 🟡 | `AddAttachment` links a fileId; **no upload pipeline / MinIO** |
| File storage (MinIO) | ❌ | Package referenced, `Storage/` folder empty, no client/abstraction |
| Realtime (SignalR) | 🟡 | `ChatHub` at `/hubs/chat` (JoinChat/LeaveChat, membership-gated); new messages pushed via outbox → `IRealtimeNotifier` (`MessageSent` event). _(M3)_ Typing/presence + Redis backplane pending; not runtime-verified |
| Messaging bus (RabbitMQ/MassTransit) | ❌ | Packages referenced; `RabbitMQ/` folder empty; not wired |
| Caching (Redis) | ❌ | Package referenced; `Redis/` folder empty; not wired |
| Presence / typing / last-seen | ❌ | `LastSeenAt` commented out in `User` |
| Notifications | ❌ | `NotificationType` enum only; `Features/Notifications/` empty |
| Search (global / messages) | ❌ | Only contact search exists |
| Logging / Serilog | ✅ | Serilog provider configured (`AddSerilogLogging`), console sink, request logging, MediatR `LoggingBehavior`. _(M0)_ |
| Observability (health/metrics/tracing) | 🟡 | Health checks at `/health`, `/health/live`, `/health/ready` (DB readiness). Metrics/tracing (OpenTelemetry) still missing. _(M0)_ |
| Domain events dispatch | ✅ | Events raised in `Message`/`Chat`; persisted to a transactional **outbox** in `SaveChangesAsync` and published at-least-once by `OutboxProcessor` → MediatR. _(M0)_ |
| Transactional outbox | ✅ | `outbox_messages` table + `OutboxProcessor` background service (polling, retry cap). Migration `AddOutboxMessages`. _(M0)_ |
| Validation pipeline | ✅ | `ValidationBehavior` registered as open MediatR behavior; validators auto-registered. _(M0)_ |
| MediatR pipeline behaviors | ❌ | `Behaviors/` folders empty (Logging/Transaction/Validation) |
| AutoMapper usage | 🟡 | Package referenced; mapping done via hand-written extension methods |
| Docker / docker-compose | ❌ | None present |
| Tests (unit/integration) | 🟡 | `tests/Messenger.UnitTests` (20 tests: domain events, validation behavior, dispatcher, outbox round-trip + EF InMemory outbox flows). No full API integration tests yet. _(M0)_ |
| README | ❌ | One-line placeholder |

## 3. Controllers

| Controller | Endpoints | Status | Missing operations |
|---|---|---|---|
| `AuthController` | request-otp, verify-otp, refresh-token, logout, revoke-session, GET sessions | ✅ | revoke-all endpoint not exposed |
| `ProfileController` | request/confirm phone change, set username, GET/PUT profile, GET/PUT privacy | ✅ | profile photo upload |
| `UserController` | GET user-profile/{id} | 🟡 | search users, resolve by username |
| `ContactController` | POST, PUT, DELETE, GET, GET search | ✅ | — |
| `BlockedUsersController` | POST/DELETE/GET | ✅ | — |
| `ChatsController` | groups, channels, **private/{userId}**, **GET my**, join, **leave (owner-transfer)**, members add/remove, **GET members**, promote/demote, GET/PUT info, mute/unmute, archive/unarchive, pin/unpin | 🟡 | saved-messages chat, channel subscriber model |
| `MessagesController` | send, edit, delete, reply, forward, read, reaction add/remove, attachment, **GET chat/{chatId}** (paged), **GET {messageId}** | 🟡 | search, pin/unpin message |

> Note: `GetChatMessages` / `GetMessage` were empty template stubs — now **implemented** and
> exposed (M1), member-gated. `GetChatMembers` now loads participants via a real projection
> (the prior handler relied on an unloaded navigation). See `api-reference.md`.

## 4. CQRS inventory

~50 command/query handlers exist across Auth, Users (Profile/Privacy/Blocking), Contacts,
Chats, Messages. All implement `IAppRequestHandler` and return `Result`/`Result<T>`.
Full table in `api-reference.md`.

## 5. Known correctness issues found during audit

1. ~~**`SendMessageHandler` dead guard**~~ — **FIXED (M0):** replaced with `Chat.IsParticipant(userId)`.
2. ~~**Domain events never raised**~~ — **FIXED (M0):** events raised in `Message`/`Chat` and
   dispatched from `ApplicationDbContext.SaveChangesAsync` via `IDomainEventDispatcher`.
3. ~~**No validation execution**~~ — **FIXED (M0):** `ValidationBehavior` registered in the MediatR pipeline.
4. **Migrations folder typo** — physical folder `Persistence/Migraions/`; csproj declares empty
   `Migrations/`. Cosmetic but confusing.
5. ~~**Soft-delete query filter disabled**~~ — **FIXED (M0):** global `HasQueryFilter` re-enabled
   for all `SoftDeletableEntity<Guid>` types; deleted rows are excluded by default
   (`IgnoreQueryFilters()` to read them).
6. **Mixed error strategy** — handlers both return `Result.Failure` and throw exceptions
   (`Domain*/NotFound/Forbidden`); `ExceptionHandlingMiddleware` catches the latter.
7. **No transaction/outbox** — multi-aggregate writes are not transactional beyond a single
   `SaveChanges`; no outbox for reliable event/integration publishing.

See `architecture.md` §Issues for severity and fixes.
