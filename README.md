# Messenger

[![CI](https://github.com/Mistogan1991/Messenger/actions/workflows/ci.yml/badge.svg)](https://github.com/Mistogan1991/Messenger/actions/workflows/ci.yml)

A Telegram-inspired messenger backend built with **.NET 10**, **ASP.NET Core**, Clean
Architecture, DDD, CQRS (MediatR), EF Core + PostgreSQL, JWT auth, and a transactional outbox.
Object storage runs on MinIO; Redis and RabbitMQ are provisioned for upcoming realtime and
fan-out features.

## Architecture

Six projects under Clean Architecture (dependencies point inward):

| Project | Responsibility |
|---|---|
| `Messenger.Domain` | Aggregates, value objects, domain events |
| `Messenger.Application` | CQRS commands/queries + handlers, abstractions |
| `Messenger.Infrastructure` | Auth (JWT/OTP), MinIO storage, logging |
| `Messenger.Persistence` | EF Core DbContext, repositories, migrations, outbox |
| `Messenger.API` | Controllers, middleware, composition root |
| `Messenger.Contracts` | Shared contracts |

See [`docs/`](docs/) for the architecture, domain model, API reference, roadmap, and current
implementation status.

## Run with Docker

Brings up the API plus PostgreSQL, MinIO, Redis, and RabbitMQ. EF Core migrations are applied
automatically on startup.

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| Health | http://localhost:8080/health · `/health/live` · `/health/ready` |
| MinIO console | http://localhost:9001 (minioadmin / minioadmin) |
| RabbitMQ UI | http://localhost:15672 (guest / guest) |

## Run locally

Requires the .NET 10 SDK and a reachable PostgreSQL (connection string in
`API/appsettings.Development.json`).

```bash
dotnet test                      # run the unit test suite
dotnet run --project API         # start the API
```

## Tests

```bash
dotnet test
```
