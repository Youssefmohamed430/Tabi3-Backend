# Backend architecture

The backend is a modular monolith with inward dependencies:

```text
Tabi3-Backend (HTTP host) -> Application -> Domain
Tabi3-Backend (composition root) -> Infrastructure -> Application -> Domain
```

- `Domain` owns business entities, value objects, and domain rules. It has no EF Core or HTTP dependency.
- `Application` owns use cases, DTOs, feature services, and persistence contracts.
- `Infrastructure` implements persistence and external integrations. EF Core mappings belong beside their feature in this project and are loaded by `Tabi3DbContext`.
- `Tabi3-Backend` owns controllers, middleware, configuration, and dependency registration.

## Persistence patterns

- Repositories provide the persistence boundary. The initial generic `IRepository<TEntity>` is intentionally small; introduce feature or aggregate-specific repository contracts when real query or domain behavior requires them.
- Queries use [Ardalis.Specification](https://github.com/ardalis/Specification). Put filters, includes, sorting, and paging in named specifications close to their feature. Keep tenant scope in every tenant-owned query; do not accept a tenant identifier from an untrusted request as proof of access.
- Every persisted entity that supports deletion implements `Domain.Common.ISoftDeletable`, which contributes a non-nullable `IsDeleted` boolean column. `IRepository<TEntity>.Remove` sets this flag; EF Core's global query filter hides flagged rows by default. Use `IgnoreQueryFilters` only in an explicit recovery/admin use case, and combine future tenant filters with the soft-delete filter instead of replacing it.
- `IUnitOfWork` commits tracked changes through one `SaveChangesAsync` call. A use case that changes multiple aggregates should save once after all domain checks pass.
- Explicit database transactions are available through `BeginTransactionAsync`, `CommitAsync`, and `RollbackAsync`. `CommitAsync` saves tracked changes before committing; `RollbackAsync` discards the active transaction. Both close the transaction afterward, and nested transactions are rejected.
- Register feature services directly with dependency injection. Add a service manager only if a concrete use case demonstrates that an orchestration facade improves the boundary; an empty wrapper around services is not useful at this stage.

## Configuration

Set `ConnectionStrings:DefaultConnection` using user secrets or the deployment environment before resolving persistence services. `AddInfrastructure` leaves the DbContext unregistered when the connection string is absent so the API can run as an empty scaffold without embedding a credential. The first database-backed feature should add its entity configuration and verify tenant isolation before exposing routes.

The existing target is .NET 9 with matching EF Core 9 and Npgsql 9 packages. Review framework and provider support before changing target versions.
