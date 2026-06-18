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
| OpenTelemetry traces + metrics | Full observability | Serilog | Med | M | ⏳ Next |
| Integration tests (Testcontainers) | End-to-end safety | unit tests | Med | M | ⏳ Next |

## Milestone 1 — Core chat completeness
| Task | Why | Depends on | Risk | Effort |
|---|---|---|---|---|
| `StartPrivateChat` command + endpoint | Cannot DM today | M0 | Low | S |
| `GetMyChats` (chat list w/ last message, unread) | Primary app screen | read models | Med | M |
| Expose `GetChatMessages` / `GetMessage` / `GetChatMembers` endpoints | Queries exist, unexposed | — | Low | S |
| Owner-leave ownership transfer rule | Invariant gap | M0 | Low | S |
| Channel posting permission rules | Channels are half-built | M0 | Med | M |

## Milestone 2 — Messaging features
| Task | Why | Depends on | Risk | Effort |
|---|---|---|---|---|
| MinIO storage abstraction + pre-signed upload/download | Attachments need real files | M0 | Med | M |
| Wire `File` aggregate to upload pipeline | Metadata persistence | MinIO | Low | S |
| Voice messages / media types | Telegram parity | MinIO | Med | M |
| Per-message delivery + seen receipts | Status model beyond last-read | events | Med | M |

## Milestone 3 — Realtime
| Task | Why | Depends on | Risk | Effort |
|---|---|---|---|---|
| SignalR hub (message/typing/presence) | Core realtime UX | M0 events | Med | L |
| Redis backplane for SignalR | Scale-out | SignalR, Redis | Med | M |
| Presence + last-seen (Redis) | Re-enable `LastSeenAt` | Redis | Med | M |
| Typing indicators | UX | SignalR | Low | S |

## Milestone 4 — Distributed infrastructure
| Task | Why | Depends on | Risk | Effort |
|---|---|---|---|---|
| RabbitMQ/MassTransit + outbox consumers | Decoupled fan-out, notifications | M0 outbox | High | L |
| Notifications service (push/in-app) | Engagement | Rabbit, events | Med | M |
| Redis caching (profiles, chat lists) | Latency at scale | Redis | Med | M |
| Global search (messages/users/chats) | Discovery | read models | High | L |

## Milestone 5 — Scale & production readiness
| Task | Why | Depends on | Risk | Effort |
|---|---|---|---|---|
| Docker + docker-compose (api, pg, redis, rabbit, minio) | Repro/dev/prod parity | — | Low | M |
| OpenTelemetry traces + metrics | Observability | Serilog | Med | M |
| Rate limiting / abuse protection (Redis) | Safety at scale | Redis | Med | M |
| Message partitioning / read-model store | Millions of users | M4 | High | L |
| CI pipeline (build/test/migrate) | Quality gate | tests | Low | M |

## Recommended next feature
**M0 → Domain event dispatch + validation pipeline** is the highest-leverage starting point:
small, low-risk, and a hard prerequisite for realtime, notifications, and read models. See the
cycle plan in the audit response / `implementation-status.md`.
