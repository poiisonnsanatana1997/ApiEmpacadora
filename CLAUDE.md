# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core 8 REST API for a produce packing/warehousing company ("Empacadora"). Manages the full supply chain: supplier orders → receiving → classification → pallets (tarimas) → client orders → shipment. Uses SQL Server with EF Core and JWT authentication.

## Commands

```bash
# Build
dotnet build

# Run (uses IIS Express profile by default; app runs at http://localhost:57664)
dotnet run

# Swagger UI (always enabled, all environments)
# http://localhost:57664/swagger

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Add a NuGet package
dotnet add package <PackageName>
```

There are no automated tests in this project.

## Architecture

Layered architecture with these conventions:

```
Controllers/          → HTTP layer, no business logic, extracts User.Identity.Name from JWT
Services/             → Business logic; interfaces are in Services/Interfaces/ (some older interfaces live directly in Services/)
Repositories/         → Interfaces only (Repositories/Interfaces/)
Infrastructure/Data/  → ApplicationDbContext (EF Core)
Infrastructure/Repositories/ → Repository implementations
Models/Entities/      → EF Core domain entities (Spanish naming)
Models/DTOs/          → Data Transfer Objects
Models/Exceptions/    → Custom exception types
Middleware/           → ExceptionHandlingMiddleware (global error handling)
```

**Dependency flow:** Controller → IService → IRepository → ApplicationDbContext

**Service registration** (all in `Program.cs`):
- Repositories and Services: `AddScoped`
- `ILoggingService`: `AddSingleton` (used by middleware)

## Key Patterns

**Authorization:** All write endpoints use `[Authorize]`. GET endpoints are generally public. Username is extracted in controllers with `User.Identity.Name` and passed down to service methods.

**Transactions:** Complex multi-entity operations use `_context.Database.BeginTransactionAsync()` with try/catch rollback inside services. Some services inject `ApplicationDbContext` directly alongside their repository for complex queries.

**Circular dependency workaround:** `TarimaService` and others inject `IServiceProvider` and resolve circular dependencies lazily (e.g., `_serviceProvider.GetRequiredService<IPedidoClienteService>()`).

**DTO mapping:** Done manually in service methods — no AutoMapper. Both entity→DTO and DTO→entity conversions happen in the service layer.

**Exception handling:** Throw custom exceptions (`EntityNotFoundException`, `BusinessRuleException`, `ValidationException`, `DatabaseException`, `FileOperationException`) from services; `ExceptionHandlingMiddleware` maps these to appropriate HTTP status codes and structured JSON responses.

**Culture:** Application forces `en-US` culture globally (decimal point, not comma).

## Domain Model Highlights

- **Tarima**: A pallet. Auto-generated code format: `TAR-{yyyyMMdd}-{nnn}`. Has status (Parcial/Completa). Links to `Clasificacion` via `TarimaClasificacion` (join table with Peso, Tipo, Cantidad).
- **Clasificacion**: A classification/processing batch from a supplier order (`PedidoProveedor`). Links to tarimas, mermas (waste), and retornos (returns).
- **PedidoCliente**: Client order with `PorcentajeSurtido` (fill percentage) that is recalculated automatically whenever tarimas are assigned/unassigned/updated.
- **PedidoTarima**: Join table between `PedidoCliente` and `Tarima`.
- **OrdenPedidoCliente**: Line items of a client order (Tipo + Producto + Cantidad/Peso).
- **TarimaResumenDiario**: Pre-computed daily summary table (unique on Fecha+Tipo), maintained by `TarimaPesoService.ProcesarResumenDiarioAsync()` which is called after tarima creation.
- **CajaCliente**: Box types belonging to a client.
- **Caja**: Boxes linked to a `Clasificacion`, with adjustable quantity.

Legacy entity `Product` (English) exists in `Models/Entities/` but is commented out of `ApplicationDbContext`. The active product entity is `Producto` (Spanish).

## Configuration

Connection string and JWT settings live in `appsettings.json`:
- DB: `ConnectionStrings:DefaultConnection` — SQL Server, local instance named `AppAPIEmpacadora`
- JWT: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryInMinutes` (default 480 min)

Uploaded files (provider fiscal documents, product images) are stored in `wwwroot/uploads/` with GUID-prefixed filenames and served as static files.

## Middleware Order (critical)

1. `UseSwagger` / `UseSwaggerUI`
2. `UseHttpsRedirection`
3. `UseStaticFiles`
4. `UseMiddleware<ExceptionHandlingMiddleware>`
5. OPTIONS preflight handler (inline middleware)
6. `UseCors("PermitirTodo")`
7. `UseAuthentication`
8. `UseAuthorization`
9. `MapControllers`
