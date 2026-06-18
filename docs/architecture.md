# Architecture

> Audit date: 2026-06-18

## 1. Overview

Messenger is a Telegram-inspired backend built with **.NET 10 / ASP.NET Core Web API** following
**Clean Architecture**, **DDD**, and **CQRS (MediatR)**. Persistence is **EF Core + PostgreSQL**.
Authentication is **JWT + refresh tokens** with per-device sessions and **OTP** sign-in.

```
            ┌──────────────────────────────────────────────┐
            │                 API (Web)                    │  Controllers, Contracts, Mappings,
            │  ASP.NET Core · JWT · Swagger · Middleware   │  ExceptionHandlingMiddleware
            └───────────────┬──────────────────────────────┘
                            │ MediatR (IRequest<Result>)
            ┌───────────────▼──────────────────────────────┐
            │              Application                     │  Commands/Queries + Handlers,
            │  CQRS · Result<T> · Abstractions · Validators│  DTOs, repository/UoW interfaces
            └───────────────┬──────────────────────────────┘
                            │ depends on
            ┌───────────────▼──────────────────────────────┐
            │                 Domain                       │  Aggregates, Entities, VOs,
            │  Pure · No infra refs · Domain Events        │  Domain Events, Enums
            └──────────────────────────────────────────────┘
              ▲                                  ▲
   ┌──────────┴─────────┐              ┌─────────┴───────────┐
   │   Persistence      │              │   Infrastructure    │
   │ EF Core · Npgsql   │              │ JWT/OTP/Hash/CurrentUser
   │ Repos · UoW · Migr.│              │ (MinIO/Redis/Rabbit  empty)
   └────────────────────┘              └─────────────────────┘
```

## 2. Layering rules (as currently enforced)

- **Domain** references nothing outward. ✅
- **Application** references Domain + Contracts only. ✅
- **Persistence/Infrastructure** reference inward and implement Application abstractions. ✅
- **API** is the composition root (`AddApplicationServices` / `AddInfrastructureServices` /
  `AddPersistenceServices` / `AddApiServices`). ✅

## 3. Cross-cutting building blocks

- **Result pattern** (`Application/Common/Models/Result.cs`) — `Result` / `Result<T>` with
  `IsSuccess`, `Errors`, optional `ErrorCode`.
- **CQRS markers** — `IAppRequest`, `IAppRequest<T>`, `IAppRequestHandler<...>` over MediatR.
- **Exception handling** — `ExceptionHandlingMiddleware` translates `Domain/NotFound/Forbidden/
  Unauthorized/Validation` exceptions to HTTP responses.
- **Auth** — `IJwtProvider`, `IOtpProvider`, `IRefreshTokenGenerator`, `IHashProvider` (SHA-256),
  `ICurrentUser` (reads claims from `HttpContext`).
- **Persistence** — `IUnitOfWork` wraps `SaveChangesAsync`; one repository per aggregate root.

## 4. Architecture & quality issues

Severity: 🔴 High · 🟠 Medium · 🟡 Low

| # | Issue | Sev | Explanation | Recommended fix |
|---|---|---|---|---|
| A1 | ~~Domain events not dispatched~~ | ✅ Done (M0) | Events raised in `Message`/`Chat`. Domain stays MediatR-free: aggregates raise pure `IDomainEvent`; `DomainEventDispatcher` wraps each in `DomainEventNotification<T>` (`INotification`) and publishes via `IPublisher`. Delivery is now via the **transactional outbox** (see A4), not in-process post-commit. | — |
| A2 | ~~Broken membership guard in `SendMessageHandler`~~ | ✅ Done (M0) | Replaced unreachable `GetParticipant(...) is null` with `Chat.IsParticipant(userId)`. | — |
| A3 | ~~No validation pipeline~~ | ✅ Done (M0) | `ValidationBehavior<TReq,TRes>` registered as an open MediatR behavior; validators auto-registered by assembly scan. | — |
| A4 | ~~No transactional consistency / outbox~~ | ✅ Done (M0) | Domain events are now written to an `outbox_messages` table in the **same transaction** as the aggregate changes (`ApplicationDbContext.SaveChangesAsync`), then published **at-least-once** by `OutboxProcessor` (background service, polled, retry-capped). Supersedes the previous post-commit in-process dispatch. A MediatR `TransactionBehavior` wrapping multi-step commands is still a smaller follow-up. | — |
| A5 | Soft-delete filter disabled | 🟠 | Global query filter commented out; deleted entities leak into reads. | Re-enable the `HasQueryFilter` loop with an `IsDeleted` shadow/property convention. |
| A6 | Mixed error model | 🟠 | Handlers both return `Result.Failure` and throw; inconsistent for callers and tests. | Pick one: prefer `Result` for expected failures, exceptions only for truly exceptional/forbidden. |
| A7 | Anemic-ish edges | 🟡 | Some chat/message rules (channel post permissions, owner-leave transfer, edit-window) live nowhere or partly in handlers. | Push remaining rules into aggregates (`Chat`, `Message`). |
| A8 | Repository read leaks | 🟡 | Verify repos don't expose `IQueryable`; queries should be encapsulated or use read models. | Keep repos task-returning; add dedicated read queries/projections. |
| A9 | No infra for scale targets | 🟠 | SignalR/Redis/RabbitMQ/MinIO referenced but unimplemented; "millions of users" claims unmet. | Implement per roadmap M3–M5. |
| A10 | No tests | 🟠 | Zero unit/integration tests; refactors are unsafe. | Add `Domain.UnitTests`, `Application.UnitTests`, `Api.IntegrationTests` (Testcontainers). |
| A11 | ~~No structured logging / observability~~ | 🟡 Partial (M0) | **Done:** Serilog provider + console sink + request logging + MediatR `LoggingBehavior`; health checks at `/health`, `/health/live`, `/health/ready`. **Remaining:** OpenTelemetry traces/metrics, config-driven Serilog (`ReadFrom.Configuration`). | Add OpenTelemetry next. |
| A12 | Nullable warnings on aggregates | 🟡 | 13 CS8618 warnings (EF-only ctors). | Suppress per-member with `= null!` or `required`, document the EF-ctor pattern. |

## 5. Target architecture (where we're heading)

- Domain events → in-process `INotification` handlers for read models + realtime.
- Integration events → **outbox** → RabbitMQ (MassTransit) → consumers (notifications, fan-out).
- **SignalR** hub for realtime delivery, presence, typing; Redis backplane for scale-out.
- **Redis** for presence, last-seen, rate limiting, hot caches.
- **MinIO** for media; pre-signed upload/download; `File` aggregate tracks metadata.
- Observability: Serilog + OpenTelemetry traces/metrics + health checks.
