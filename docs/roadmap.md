# Roadmap

> Audit date: 2026-06-18. Effort: S (≤1d) · M (2–4d) · L (1–2w). Risk: Low/Med/High.
> Sequenced so foundational concerns (events, validation, transactions) land before the
> distributed/realtime features that depend on them.

## Milestone 0 — Foundation hardening (prereq for everything)
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| Domain event dispatch (`IDomainEventDispatcher` from `SaveChanges`) | Unblocks realtime, notifications, read models | — | Med | M | ✅ Done |
| Re-enable `RaiseDomainEvent` call sites | Events are dead code today | dispatcher | Low | S | ✅ Done |
| `ValidationBehavior` + register validators | Commands reach handlers unvalidated | — | Low | S | ✅ Done |
| Fix `SendMessage` membership guard (`IsParticipant`) | Correctness/security bug | — | Low | S | ✅ Done |
| Unit test project (domain events, validation, dispatcher, outbox) | Safe refactoring | — | Med | M | 🟡 Started (20 tests) |
| Transactional outbox + `OutboxProcessor` | Atomicity + reliable at-least-once publish | dispatcher | Med | M | ✅ Done |
| Serilog + request logging + `LoggingBehavior` + `/health` | Operability baseline | — | Low | S | ✅ Done |
| `TransactionBehavior` (multi-step commands) | Wrap command handlers in a transaction | outbox | Low | S | ⏳ Next |
| Re-enable soft-delete query filter | Deleted rows leak into reads | — | Low | S | ✅ Done |
| OpenTelemetry traces + metrics | Full observability | Serilog | Med | M | ✅ Done (M5) |
| Integration tests (Testcontainers) | End-to-end safety | unit tests | Med | M | ⏳ Next |

## Milestone 1 — Core chat completeness
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| `StartPrivateChat` command + endpoint | Cannot DM today | M0 | Low | S | ✅ Done |
| `GetMyChats` (chat list w/ last-message preview + per-user flags) | Primary app screen | read models | Med | M | ✅ Done (unread count deferred — needs Postgres-validated query) |
| `GetChatMessages` / `GetMessage` / `GetChatMembers` read endpoints | Read APIs missing | — | Low→Med | M | ✅ Done (GetChatMessages/GetMessage were empty stubs — built, not just wired; all member-gated) |
| Owner-leave ownership transfer rule | Invariant gap | M0 | Low | S | ✅ Done (`Chat.Leave` — admin-first, else oldest member; chat never ownerless) |
| Channel posting permission rules | Channels are half-built | M0 | Med | M | ✅ Done (`Chat.CanSendMessages`; enforced in Send/Reply/Forward — the latter two had no membership check before) |

## Milestone 2 — Messaging features
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| MinIO storage abstraction + pre-signed upload/download | Attachments need real files | M0 | Med | M | ✅ Done (`IFileStorage`/`MinioFileStorage`, Files API; not runtime-verified vs live MinIO) |
| Wire `File` aggregate to upload pipeline | Metadata persistence | MinIO | Low | S | ✅ Done (`RequestFileUpload` persists `File`; `FileRepository`) |
| Download authorization (chat-membership ACL) | Any authed user can mint a download URL today | MinIO | Med | S | ✅ Done (`IFileRepository.CanUserAccessAsync`: uploader / chat-member / profile photo) |
| Voice messages / media types | Telegram parity | MinIO | Med | M | ⏳ Next |
| Per-message delivery + seen receipts | Status model beyond last-read | events | Med | M | ⏳ Next |

## Milestone 3 — Realtime
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| SignalR hub — new-message push | Core realtime UX | M0 events | Med | L | ✅ Done (`ChatHub` + outbox→`IRealtimeNotifier`; not runtime-verified) |
| Typing indicators | UX | SignalR | Low | S | ✅ Done (`ChatHub.Typing`/`StopTyping` → `UserTyping`/`UserStoppedTyping`, membership-gated; not runtime-verified) |
| Redis backplane for SignalR | Scale-out | SignalR, Redis | Med | M | ✅ Done (`AddStackExchangeRedis`, opt-in via `ConnectionStrings:Redis`; compose wired) |
| Presence + last-seen (Redis) | Re-enable `LastSeenAt` | Redis | Med | M | ✅ Done (`IPresenceTracker` in-memory+Redis; hub lifecycle; `GET /user/{id}/presence`; broadcast + privacy deferred) |

## Milestone 4 — Distributed infrastructure
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| RabbitMQ/MassTransit integration events | Decoupled fan-out, notifications | M0 outbox | High | L | 🟡 In review (`feature/rabbitmq-integration-events`) |
| Notifications service (in-app feed) | Engagement | events | Med | M | ✅ Done (in-app: aggregate + fan-out on message-sent + read management; push delivery pending) |
| Redis caching (profiles, chat lists) | Latency at scale | Redis | Med | M | ⏳ Next |
| Global search (messages/users/chats) | Discovery | read models | High | L | ⏳ Next |

## Milestone 5 — Scale & production readiness
| Task | Why | Depends on | Risk | Effort | Status |
|---|---|---|---|---|---|
| Docker + docker-compose (api, pg, redis, rabbit, minio) | Repro/dev/prod parity | — | Low | M | ✅ Done (Dockerfile + compose + migrate-on-startup; not yet built/run — no Docker in dev env) |
| OpenTelemetry traces + metrics | Observability | Serilog | Med | M | ✅ Done (ASP.NET Core/HttpClient/runtime; opt-in OTLP export via `OpenTelemetry:OtlpEndpoint`) |
| Rate limiting / abuse protection | Safety at scale | Redis | Med | M | ✅ Done (built-in ASP.NET limiter: global per-client + tight OTP policy; **per-instance/in-memory** — Redis-backed distributed limiter is a follow-up) |
| Message partitioning / read-model store | Millions of users | M4 | High | L |
| CI pipeline (build/test/migrate) | Quality gate | tests | Low | M |

## Recommended next feature
**M0 → Domain event dispatch + validation pipeline** is the highest-leverage starting point:
small, low-risk, and a hard prerequisite for realtime, notifications, and read models. See the
cycle plan in the audit response / `implementation-status.md`.
