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
| Chats — channels | 🟡 | Create/join exist; **posting restricted to owner/admins** _(M1, `Chat.CanSendMessages`)_; no dedicated subscriber model yet |
| Chats — private/saved | 🟡 | `StartPrivateChat` command + endpoint added (idempotent) _(M1)_; Saved Messages still has no command |
| Chat membership ops | ✅ | Add/remove members, promote/demote admin |
| Chat per-user state | ✅ | Mute/archive/pin (on `ChatParticipant`) |
| Messaging — send/edit/delete | ✅ | Text only |
| Messaging — reply | ✅ | `ReplyToMessageId` |
| Messaging — forward | 🟡 | Aggregate + handler exist; forward author-hiding not exercised |
| Reactions | ✅ | Add/remove, unique per (message,user,emoji) |
| Read receipts | 🟡 | `MarkAsRead` sets last-read id; no per-message delivery/seen fan-out |
| Attachments | 🟡 | `AddAttachment` links a fileId; upload pipeline now exists via Files API _(M2)_ |
| File storage (MinIO) | 🟡 | `IFileStorage` + `MinioFileStorage` (pre-signed PUT/GET URLs); `RequestFileUpload`/`GetFileDownloadUrl` + `FilesController`; `FileRepository` with **download ACL** (uploader / chat-member / profile photo). _(M2)_ Not yet runtime-verified against a live MinIO |
| Realtime (SignalR) | 🟡 | `ChatHub` at `/hubs/chat` (JoinChat/LeaveChat + **Typing/StopTyping**, all membership-gated); new messages pushed via outbox → `IRealtimeNotifier` (`MessageSent`); `UserTyping`/`UserStoppedTyping` to others. _(M3)_ Presence + Redis backplane pending; not runtime-verified |
| Messaging bus (RabbitMQ/MassTransit) | ❌ | Packages referenced; `RabbitMQ/` folder empty; not wired |
| Caching (Redis) | 🟡 | Wired as the **SignalR backplane** (`AddStackExchangeRedis`, enabled when `ConnectionStrings:Redis` is set). _(M3)_ Not yet used for general caching |
| Presence / typing / last-seen | 🟡 | `User.LastSeenAt` re-enabled (migration `AddUserLastSeenAt`); `IPresenceTracker` (in-memory + Redis); hub tracks connect/disconnect, stamps last-seen on last disconnect; `GET /api/user/{id}/presence`. _(M3)_ Typing done separately; presence broadcast + last-seen privacy pending |
| Notifications | 🟡 | In-app feed: `Notification` aggregate + migration `AddNotifications`; fan-out on `MessageSentEvent` (one per member except sender); `NotificationsController` (list, unread-count, mark read / read-all). _(M4)_ Push delivery + mute-aware filtering pending |
| Search (global / messages) | ❌ | Only contact search exists |
| Rate limiting | 🟡 | Built-in ASP.NET limiter: global per-client (100/min) + tight OTP policy (5/5min) → 429. _(M5)_ Per-instance/in-memory; Redis-backed distributed limiter pending |
| Logging / Serilog | ✅ | Serilog provider configured (`AddSerilogLogging`), console sink, request logging, MediatR `LoggingBehavior`. _(M0)_ |
| Observability (health/metrics/tracing) | ✅ | Health checks _(M0)_ + **OpenTelemetry** traces & metrics (ASP.NET Core, HttpClient, runtime), OTLP export when `OpenTelemetry:OtlpEndpoint` set. _(M5)_ Not runtime-verified |
| Domain events dispatch | ✅ | Events raised in `Message`/`Chat`; persisted to a transactional **outbox** in `SaveChangesAsync` and published at-least-once by `OutboxProcessor` → MediatR. _(M0)_ |
| Transactional outbox | ✅ | `outbox_messages` table + `OutboxProcessor` background service (polling, retry cap). Migration `AddOutboxMessages`. _(M0)_ |
| Validation pipeline | ✅ | `ValidationBehavior` registered as open MediatR behavior; validators auto-registered. _(M0)_ |
| MediatR pipeline behaviors | ❌ | `Behaviors/` folders empty (Logging/Transaction/Validation) |
| AutoMapper usage | 🟡 | Package referenced; mapping done via hand-written extension methods |
| Docker / docker-compose | 🟡 | `Dockerfile` + `docker-compose.yml` (api, postgres, minio, redis, rabbitmq); migrate-on-startup. _(M5)_ **Not built/run here — no Docker in dev env** |
| Tests (unit/integration) | 🟡 | `tests/Messenger.UnitTests` (20 tests: domain events, validation behavior, dispatcher, outbox round-trip + EF InMemory outbox flows). No full API integration tests yet. _(M0)_ |
| README | ✅ | Overview, architecture table, Docker + local run instructions. _(M5)_ |

## 3. Controllers

| Controller | Endpoints | Status | Missing operations |
|---|---|---|---|
| `AuthController` | request-otp, verify-otp, refresh-token, logout, revoke-session, GET sessions | ✅ | revoke-all endpoint not exposed |
| `ProfileController` | request/confirm phone change, set username, GET/PUT profile, GET/PUT privacy | ✅ | profile photo upload |
| `UserController` | GET user-profile/{id}, **GET {id}/presence** | 🟡 | search users, resolve by username |
| `ContactController` | POST, PUT, DELETE, GET, GET search | ✅ | — |
| `BlockedUsersController` | POST/DELETE/GET | ✅ | — |
| `ChatsController` | groups, channels, **private/{userId}**, **GET my**, join, **leave (owner-transfer)**, members add/remove, **GET members**, promote/demote, GET/PUT info, mute/unmute, archive/unarchive, pin/unpin | 🟡 | saved-messages chat, channel subscriber model |
| `MessagesController` | send, edit, delete, reply, forward, read, reaction add/remove, attachment, **GET chat/{chatId}** (paged), **GET {messageId}** | 🟡 | search, pin/unpin message |
| `FilesController` | POST upload-url, GET {id}/download-url (ACL-gated) | ✅ _(M2)_ | — |
| `NotificationsController` | GET (list), GET unread-count, POST {id}/read, POST read-all | ✅ _(M4)_ | push delivery |

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
8. **Transitive `MessagePack` advisory (NU1903, GHSA-hv8m-jj95-wg3x)** — via
   `Microsoft.AspNetCore.SignalR.StackExchangeRedis`. **No patched MessagePack release exists**
   (2.5.x and 3.1.x are all flagged), so it can't be upgraded away. **Risk accepted & suppressed**
   (`NuGetAuditSuppress` in `API/Messenger.API.csproj` with justification): MessagePack is only
   used to serialize SignalR messages over the **internal Redis backplane** (data produced by our
   own instances, not end-user input). Mitigation: keep Redis network-isolated. Remove the
   suppression when a fixed release ships. _(M3)_

See `architecture.md` §Issues for severity and fixes.
