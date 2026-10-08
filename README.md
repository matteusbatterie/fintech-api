# FinTech Ledger API

![CI](https://github.com/matteusbatterie/fintech-api/actions/workflows/ci.yml/badge.svg)

A double-entry ledger API built with **.NET 10**, **Domain-Driven Design** and **Clean Architecture**.

This is a portfolio project. The goal is not feature count but validity under the conditions a real ledger faces: invariants that can't be bypassed, money that never disappears halfway through a transfer, retried requests that don't double-apply, and concurrent writes that don't silently overwrite each other.

## Highlights

- **Rich domain model.** `Account` and `Transaction` are aggregates that protect their own invariants (no overdraft, currency match, no unbalanced postings). State changes only through methods like `Deposit()`, `Withdraw()` and `Post()`, never through public setters.
- **Double-entry bookkeeping.** Every transfer is a `Transaction` made of balanced debit and credit `Entry` rows. `Post()` refuses to complete unless debits equal credits.
- **Value objects.** `Money` and `Document` (CPF/CNPJ with checksum validation through a strategy + factory) carry their own validation.
- **Domain events.** Aggregates raise events (`AccountOpened`, `FundsDeposited`, `FundsWithdrawn`, `TransactionPosted`). They are dispatched by the Unit of Work only after a successful save, so an event never fires for a change that didn't persist.
- **Idempotency.** Money-moving endpoints require an `Idempotency-Key` header. A MediatR pipeline behavior stores a hash of the request, and replays the stored response for a true retry. It rejects a reused key with a different payload. The key reservation, the handler's writes and the completion update share one database transaction.
- **Optimistic concurrency.** `Account` carries a SQL Server `rowversion` token. A conflicting write surfaces as a `ConcurrencyConflictException` instead of a lost update.
- **CQRS with MediatR.** Commands and queries each have one handler. Handlers orchestrate; business rules live in the domain.
- **Consistent error contract.** Validation (FluentValidation) runs before idempotency in the pipeline. A global exception handler maps domain exceptions to `ProblemDetails` (RFC 7807).

## Architecture

```
src/
  FinTech.Domain/          Entities, aggregates, value objects, domain events, domain services, interfaces
  FinTech.Application/     Commands, queries, handlers, validators, pipeline behaviors
  FinTech.Infrastructure/  EF Core, repositories, Unit of Work, idempotency, event dispatcher
  FinTech.API/             Minimal API endpoints, exception handling, composition root
tests/
  FinTech.Domain.Tests/        Unit tests for the domain
  FinTech.IntegrationTests/    End-to-end tests against a real SQL Server (Testcontainers)
```

Dependencies point inward: the domain references nothing, and infrastructure implements interfaces the domain defines.

### Request Pipeline

```
HTTP request
  -> Endpoint               (reads Idempotency-Key header)
  -> ValidationBehavior     (rejects malformed input before touching the database)
  -> IdempotencyBehavior    (money-moving commands only)
  -> Command handler        (loads aggregates, calls domain methods)
  -> Unit of Work           (single commit, then dispatches domain events)
```

## API

| Method | Route | Idempotency-Key |
|--------|-------|-----------------|
| `POST` | `/api/accounts` | no |
| `GET`  | `/api/accounts/{id}` | no |
| `POST` | `/api/accounts/{id}/deposit` | required |
| `POST` | `/api/accounts/{id}/withdraw` | required |
| `POST` | `/api/transactions/transfer` | required |

In Development, interactive docs are served by Scalar at `/scalar/v1`.

## Running locally

Prerequisites: .NET 10 SDK, Docker.

```bash
# 1. Create your local config and set a strong DB_PASSWORD
cp .env.example .env

# 2. Start SQL Server
docker compose up -d db

# 3. Apply migrations
dotnet ef database update --project src/FinTech.Infrastructure --startup-project src/FinTech.API

# 4. Run the API
dotnet run --project src/FinTech.API
```

`.env` is gitignored. Only the `.env.example` template is committed.

## Tests

```bash
dotnet test FinTech.Ledger.slnx
```

- **Unit tests** cover aggregate invariants, domain events, the transfer flow and document validators.
- **Integration tests** start a throwaway SQL Server with Testcontainers, apply the real migrations, and drive the API over HTTP (open accounts, deposit, transfer, verify balances). Docker must be running.

GitHub Actions runs build and tests on every push and pull request to `main`.

## Design decisions worth knowing

- **`Entry` is a regular entity inside the `Transaction` aggregate.** It was first modeled as an EF owned type. It was promoted to its own table with a required foreign key because ledger entries need to be queried independently (statements, audits). The aggregate boundary is unchanged: there is no `DbSet<Entry>`, and entries can only be created through `Transaction.AddEntry()`.
- **Value objects use EF Core `ComplexProperty`.** `Money` and `Document` have no identity, so complex types fit better than owned entities. Each `Entry` copies the `Money` it receives, because EF tracks these instances by reference and sharing one between two entries breaks persistence.
- **Concurrent duplicate requests fail fast.** If a request arrives while another with the same idempotency key is still running, it is rejected rather than made to wait. This is a deliberate simplification at this scale.
- **A failed request doesn't burn its idempotency key.** If the handler throws, the whole transaction, including the key reservation, rolls back, so the client can retry with the same key after fixing the cause.

## Scope and what's next

Events are handled in-process (the dispatcher currently logs them). I deliberately did not add a message broker or the outbox pattern, because there is no external consumer to justify them yet.

Natural next steps:

- Transactional outbox plus Kafka or RabbitMQ for integration events
- A consumer service (read-model projection or audit trail)
- Account statements built on the independently queryable `Entries` table
- Additional support for citizenship documents (SSN, passport, etc.) and currencies (USD, EUR, etc.), including FX rates and conversion
