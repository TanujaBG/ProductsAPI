# 📚 .NET Full-Stack on Azure — 7-Day Learning Journal

A personal learning track to become a **.NET full-stack developer on Azure in 7 days**.
This folder records daily progress and the roadmap.

- **Hands-on project:** `ProductsApi` (this repo) — a minimal-API REST service.
- **Learning style:** one topic at a time → *concept → hands-on → common pitfalls → 10 interview Q&A*.

---

## 🗺️ 7-Day plan (12 topics)

| Day | Topic | Focus | Status |
|-----|-------|-------|--------|
| 1 | 1 · C# & ASP.NET Core fundamentals | records, patterns, async, LINQ, NRT, DI, middleware, hosting | ✅ Done |
| 1 | 2 · Building REST APIs | CRUD, validation, ProblemDetails, versioning, Swagger | ✅ Done |
| 2 | 3 · EF Core + Azure SQL | DbContext, migrations, relationships, tracking, perf | ✅ Done |
| 2 | 4 · AuthN/AuthZ with Entra ID | JWT, OAuth2, OIDC, policies, scopes & roles | ✅ Done |
| 3 | 5 · Frontend integration (React) | consume the API, CORS, tokens, state | 🔄 In progress |
| 3 | 6 · Azure hosting options | App Service, Container Apps, AKS, Functions | ✅ Done |
| 4 | 7 · Azure Storage | Blob, Queue, Table; SDK, SAS tokens, lifecycle | ⏳ Planned |
| 4 | 8 · Event-driven design | Service Bus / Event Grid / Event Hubs | ⏳ Planned |
| 5 | 9 · Caching & performance | Redis, in-memory/distributed, response/output caching | ⏳ Planned |
| 5 | 10 · Observability | App Insights, ILogger, OpenTelemetry, metrics, KQL | ⏳ Planned |
| 6 | 11 · CI/CD | GitHub Actions **+** Azure DevOps Pipelines | ⏳ Planned |
| 7 | 12 · IaC & architecture | Bicep (+ Terraform), Well-Architected, cost | ⏳ Planned |

> ✅ **Full 12-topic plan confirmed (2026-07-20).** Days 1–2 ✅ · Day 3: Topic 6 ✅, Topic 5 🔄 (frontend built; auth-in-UI + state pending).

---

## ✅ Progress log

### Day 1 — Fundamentals & REST APIs ✅

**Topic 1 · C# & ASP.NET Core fundamentals**
- **Records** — value equality, immutability, `with`-expressions.
- **Pattern matching** — switch expressions; relational / property / type patterns; `and`/`or`/`not`.
- **async/await** — `Task<T>`, the thread pool, `Task.WhenAll`, `Task.Run` vs `Parallel.For`, race conditions & `Interlocked`.
- **LINQ** — deferred execution, `IEnumerable` vs `IQueryable` vs `IGrouping`.
- **Nullable reference types** — `?`, `?.`, `??`, `??=`, `!`, compiler flow analysis.
- **Dependency injection** — Transient / Scoped / Singleton, the captive-dependency trap.
- **Hosting model** — generic host, `CreateBuilder`, two-phase startup, environments, Kestrel, graceful shutdown.
- **Middleware pipeline** — the onion model, order matters, authN before authZ.
- **Configuration** — layered providers (last wins), the Options pattern, user secrets / Key Vault.
- **Bonus (real codebase)** — outbound service-to-service auth & Workload Identity Federation (`TokenCredential`, Managed Identity).
- **Artifact:** `Practice` console app (`Records.cs`, `PatternMatching.cs`, `AsyncDemo.cs`, `NullableRefTypes.cs`).

**Topic 2 · Building REST APIs** — built `ProductsApi` (minimal API, `net10.0`)
- Full **CRUD** with correct status codes and **idempotency** (POST vs PUT/DELETE).
- **Validation** via data annotations → **ProblemDetails** (RFC 7807).
- Reusable **`ValidationFilter : IEndpointFilter`** (endpoint-scoped middleware).
- **Swagger / OpenAPI** (Swashbuckle), gated to Development.
- **API versioning** — URL-path `v1` / `v2` via `MapGroup`.
- Controllers vs minimal APIs comparison + 10 interview Q&A.

### Day 2 — Data & Security ✅

**Topic 3 · EF Core + Azure SQL**
- EF Core + **SQLite** (one-line swap to Azure SQL via `UseSqlServer`).
- `DbContext`, `DbSet`, **migrations** (`dotnet-ef`).
- Entities **`Product`** & **`Category`** with a one-to-many relationship.
- **Change tracking** vs `AsNoTracking`; the **N+1** problem; `Include` vs projection.
- PK/FK conventions & indexes; **pagination** (`Skip`/`Take` → `OFFSET`/`LIMIT`).
- **Transactions** (SaveChanges atomicity + explicit `BeginTransaction`); dev SQL logging.
- **Security fix** — patched a high-severity SQLite native-lib advisory.
- Verified live across all endpoints + 10 interview Q&A.

**Topic 4 · AuthN/AuthZ with Entra ID**
- **JWT bearer authentication** (`AddJwtBearer`, `TokenValidationParameters`).
- Dev **`/dev/token`** minter → test auth locally without Entra.
- **Authentication vs authorization** (401 vs 403).
- **Authorization policies** — `products.write` (scope claim) on writes, `admin` (role) on DELETE.
- Claim-mapping nuances (`MapInboundClaims`, `RoleClaimType`); one-line swap to real Entra (`Authority`/`Audience`).
- Common auth mistakes + 10 interview Q&A.
- Verified live: `201` / `401` / `403` / `204`.

**Where the code lives** — the repo is now a **monorepo** under `ProductsApi.slnx`:
- `products-api/` — the .NET API (`Auth/ Models/ Contracts/ Data/ Filters/ Extensions/ Endpoints/ Migrations/`, thin `Program.cs`).
- `products-web/` — the React + TypeScript frontend (Vite).
- `Practice/` — the Day-1 C# exercises console app.

---

## 🚧 Day 3 — Frontend & Hosting

**Topic 5 · Frontend integration (React)** 🔄 — *Increment 1 done:*
- Scaffolded **React + TypeScript** (Vite); the app + API now share one solution (`.slnx`) and Git repo.
- **CORS** on the API (`AddCors`/`UseCors`) allowing the Vite origin (`localhost:5173`).
- A typed `fetch` client (`api.ts`) + `App.tsx` rendering the live product list (loading/error/data state) — verified end-to-end.
- *Remaining:* auth token handling in the UI, state management (TanStack Query), 10 Q&A.

**Topic 6 · Azure hosting options** ✅ — compared **App Service / Container Apps / AKS / Functions** (when-to-use, scaling, pricing, cold starts), a **decision matrix**, and **10 interview Q&A**; recommended **App Service** for this API. *(Optional hands-on deploy pending an Azure subscription.)*

---

## 🔜 Coming up

### Day 4 · Topic 7 — Azure Storage ⏳
**Blob, Queue, Table** storage — SDK usage from .NET, common patterns, **SAS tokens**, lifecycle management.
Hands-on: upload blobs + process a queue · best practices · 10 interview Q&A.

### Day 4 · Topic 8 — Event-driven design ⏳
**Service Bus** (queues/topics/subscriptions) vs **Event Grid** vs **Event Hubs** — when to use each.
Messaging patterns, retries, **dead-lettering**, idempotency. Hands-on: publisher/consumer in .NET · 10 interview Q&A.

### Day 5 · Topic 9 — Caching & performance ⏳
**Azure Cache for Redis**, in-memory & distributed caching, response caching, **output caching**, async best practices, horizontal scaling.
Hands-on: cache the API + measurement techniques · 10 interview Q&A.

### Day 5 · Topic 10 — Observability ⏳
**Application Insights**, Log Analytics, structured logging with `ILogger`, distributed tracing (**OpenTelemetry**), metrics, alerts.
Hands-on: instrument the API · dashboards/**KQL** queries · 10 interview Q&A.

### Day 6 · Topic 11 — CI/CD ⏳
**GitHub Actions** *and* **Azure DevOps Pipelines** — build, test, containerize, deploy to Azure.
Environments, secrets, approvals, **blue-green / slot** deployments. Hands-on: pipeline YAML built step by step · 10 interview Q&A.

### Day 7 · Topic 12 — IaC & architecture ⏳
**Bicep** (+ Terraform basics) & cloud architecture — provisioning App Service/SQL/Storage, secure design (**Key Vault, managed identities, networking**), the **Well-Architected Framework**, cost optimization.
Hands-on: Bicep deployment + a reference architecture for a full-stack app · 10 interview Q&A.

---

*Maintained alongside the `ProductsApi` project. Format per topic: concept → hands-on → pitfalls → 10 interview Q&A.*
