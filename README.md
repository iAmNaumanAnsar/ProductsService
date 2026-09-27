# Products Service

A small Products Web API (.NET 8) with JWT-secured endpoints, an EF Core /
SQLite data layer, unit + integration tests, and an Angular front end that
consumes it.

## Contents

- [`src/ProductsService.Domain`](src/ProductsService.Domain) — the `Product`
  entity and its invariants. No framework dependencies.
- [`src/ProductsService.Application`](src/ProductsService.Application) —
  DTOs, `IProductRepository`/`IProductService` interfaces, the
  `ProductService` business logic, FluentValidation rules.
- [`src/ProductsService.Infrastructure`](src/ProductsService.Infrastructure) —
  EF Core `DbContext`, the SQLite-backed `ProductRepository`, migrations,
  seed data.
- [`src/ProductsService.Api`](src/ProductsService.Api) — controllers, JWT
  bearer auth, Swagger, CORS, the anonymous `/health` endpoint.
- [`tests/ProductsService.UnitTests`](tests/ProductsService.UnitTests) —
  service/domain/validator/token-service tests against mocked dependencies.
- [`tests/ProductsService.IntegrationTests`](tests/ProductsService.IntegrationTests) —
  `WebApplicationFactory`-driven tests against the real pipeline and a
  throwaway SQLite file per run.
- [`frontend`](frontend) — Angular 17 standalone app: login page, product
  list with a colour filter, create-product form.
- [`docs/architecture.md`](docs/architecture.md) — the internal layering
  and how this service would sit in a larger event-driven system (Orders,
  Payments, an event bus).

## Running the API

```bash
cd src/ProductsService.Api
dotnet run
```

This applies EF Core migrations and seeds five demo products automatically
on startup (see `DbInitializer.SeedAsync`), into a local
`productsservice.db` SQLite file — nothing external to install.

- Swagger UI: `http://localhost:5115/swagger`
- Anonymous health check: `GET http://localhost:5115/health`
- Get a token: `POST http://localhost:5115/api/auth/token` with
  `{"username": "demo", "password": "Passw0rd!"}`
- Use the token: `Authorization: Bearer <token>` on `/api/products`

The demo account and the JWT signing key live in `appsettings.json`, clearly
marked as demo-only — in a real deployment the signing key comes from an
environment variable / secret store, and the account comes from a real user
store (ASP.NET Core Identity, Entra ID, etc.), not a config value.

### Endpoints

| Method | Route                          | Auth      | Notes                                    |
|--------|---------------------------------|-----------|-------------------------------------------|
| GET    | `/health`                       | Anonymous | Liveness/readiness check                  |
| POST   | `/api/auth/token`                | Anonymous | Exchanges demo credentials for a JWT      |
| POST   | `/api/products`                  | Bearer    | Creates a product                         |
| GET    | `/api/products`                  | Bearer    | Lists all products                        |
| GET    | `/api/products?color=Red`        | Bearer    | Lists only products of that colour        |

`color` is a query-string filter on the list endpoint rather than a second
route, since it's the same resource with an optional filter — not a
different one.

## Running the frontend

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200`, sign in with `demo` / `Passw0rd!`. The API
must be running on `http://localhost:5115` (the default `dotnet run` URL —
see `environment.ts`).

## Tests

```bash
# Backend — 26 tests (unit + integration)
dotnet test

# Frontend
cd frontend && npx ng test --watch=false --browsers=ChromeHeadless
```

CI (`.github/workflows/ci.yml`) runs both on every push/PR to `main`.

## Notable design decisions

- **Layered architecture** (Domain → Application → Infrastructure/Api) so
  `ProductService`'s business rules (duplicate-SKU rejection, colour
  filtering) are unit-testable against a mocked `IProductRepository`,
  without a database in the loop.
- **EF Core migrations, not `EnsureCreated`** — schema changes are
  versioned and reviewable, the way they'd need to be against a real
  database.
- **A real (if minimal) JWT flow** rather than skipping auth or faking it —
  `/api/auth/token` issues a signed, expiring token that the Angular app
  stores and attaches via an `HttpInterceptorFn`, and an Angular route guard
  redirects to `/login` when it's missing.
- **FluentValidation over data annotations**, invoked explicitly in the
  controller — validation rules are unit-testable in isolation
  (`CreateProductRequestValidatorTests`), and the controller's handling of
  a failed validation is visible in one place rather than happening via an
  attribute-driven pipeline.
- **A global exception-handling middleware** as a last line of defence,
  distinct from the specific `DuplicateSkuException` → 409 mapping in the
  controller — so an unanticipated exception returns a safe `application/problem+json`
  response instead of leaking a stack trace.
