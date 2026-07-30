# EF Core — Mock Interview #1 & Study Notes

**Date:** 2026-07-30 · **Focus:** weak-area drill (EF Core), after a real interview stumble on eager loading / `Include`.
**Format:** interviewer asks one question at a time → answer → assess (✅/⚠️) + model answer + follow-up.

Use this as a review sheet — the **model answers** are the reusable part.

---

## Q1 — The ways EF Core loads related data
- **Eager (`Include`)** — `db.Products.Include(p => p.Category)`: one query (JOIN), loads the **whole** related entity. `ThenInclude` for deeper levels. Use when you always need it.
- **Lazy** — auto-loads on first access of a `virtual` navigation; needs the `Proxies` package + `UseLazyLoadingProxies()`. Convenient but risks **N+1**.
- **Explicit** — `db.Entry(product).Reference(p => p.Category).LoadAsync()` (or `.Collection(...)` for a collection): load on demand, no proxies.
- **Projection (`Select`)** — `Select(p => new { p.Id, p.Name, p.Category.Name })`: only the columns you need; **not tracked**; no `Include` required. Best for read-only / DTOs.

**Soundbite:** *`Include` = the whole entity · `Select` = just the columns · lazy/explicit = load later.*
*My result:* named `Include` only; conflated `Include` with column selection (Include loads the whole entity — use projection for specific columns). ⚠️

---

## Q2 — The N+1 problem
1 query for a list of N rows, then a **separate** query per row for related data → **1 + N** round-trips. Usual cause: **lazy loading inside a loop**.
```csharp
var products = await db.Products.ToListAsync();   // 1 query
foreach (var p in products) Use(p.Category.Name); // N queries
```
**Fix:** eager `Include` (one JOIN) or a **projection**. **Detect:** EF SQL logging (`LogTo`) — watch for repeated `SELECT … WHERE Id = @p`; or MiniProfiler.
*My result:* nailed the concept, but didn't state the fix. ⚠️ (Always land the fix.)

---

## Q3 — Tracking vs `AsNoTracking`
Tracking (**default**) snapshots each returned entity in the **change tracker** so `SaveChanges` can detect changes → costs **memory + CPU**. `AsNoTracking` skips it → faster, lighter; use for **read-only** queries. Don't use it if you'll modify + save (the entity isn't tracked). Nuance: no-tracking skips **identity resolution** (possible duplicate instances) — `AsNoTrackingWithIdentityResolution()` if needed.
*My result:* good — got the recommendation; missed the concrete cost + nuance. ✅

---

## Q4 — `IQueryable` vs `IEnumerable`
`IQueryable` = a query (expression tree) the **provider translates to SQL** → filtering runs in the **database**. `IEnumerable` = LINQ-to-Objects → runs **in memory**. **Deferred execution**: nothing runs until you enumerate (`ToList`, `foreach`, `First`, `Count`). `IQueryable` does **not** cache — each enumeration re-runs the SQL.
```csharp
db.Products.Where(p => p.Price > 100).ToList();  // ✅ filter in DB
db.Products.ToList().Where(p => p.Price > 100);  // ❌ pulls ALL rows, filters in memory
```
**Rule:** keep it `IQueryable` as long as possible; don't call `ToList()` early.
*My result:* nailed the crux (DB vs memory); muddled the "caches on first fetch" part. ✅

---

## Q5 — `DbContext` lifetime & thread-safety
**Scoped** — one per HTTP request, disposed at request end (what `AddDbContext` gives). `DbContext` is **NOT thread-safe**. A **singleton** → the *"second operation started before the previous completed"* exception, an ever-growing change tracker (**memory leak**), and cross-request data bleed. For parallel/background work use **`IDbContextFactory`**.
*My result:* got Scoped + per-request disposal; missed the thread-safety statement and the singleton failure modes. ⚠️

---

## Q6 — `SaveChanges` & change tracking
On a **tracking** load EF keeps a **snapshot** of original values (state `Unchanged`). `SaveChanges` runs **`DetectChanges`**, which **diffs current vs snapshot** to find changed columns and set each entity's **state**:

| State | SQL |
|---|---|
| `Added` | `INSERT` |
| `Modified` | `UPDATE` (changed columns) |
| `Deleted` | `DELETE` |
| `Unchanged` | — |

All statements run in **one transaction** (atomic). *This is why `AsNoTracking` can't save — no snapshot, no state.*
*My result:* high-level only; missed states + `DetectChanges` + the transaction. ⚠️

---

## Q7 — Migrations
Versioned schema evolution generated from your **model**. Workflow:
```powershell
dotnet ef migrations add <Name>   # diff(model, snapshot) → Up/Down + updates snapshot
dotnet ef database update         # apply pending migrations
```
Files: the migration (`Up`/`Down`) + the **model snapshot** (diff cache). EF records applied migrations in the **`__EFMigrationsHistory`** table and only runs the pending ones. Deploy with `migrations script --idempotent` or `Database.Migrate()`.
*My result:* didn't know it — now learned. 📚

**Source of truth (my follow-up):** model = *intent* · migrations = *the versioned recipe that runs* · snapshot = *diff cache (not truth)* · database + `__EFMigrationsHistory` = *actual state of one DB*.

---

## Q8 — Optimistic concurrency
Default `UPDATE` keys on the PK only → **last-write-wins** (silent overwrite). Add a **concurrency token**:
```csharp
[Timestamp] public byte[] RowVersion { get; set; }
```
EF then emits `… WHERE Id = @id AND RowVersion = @original`; if the row changed underneath you → **0 rows** → **`DbUpdateConcurrencyException`**, which you catch and resolve (reload / client-wins / store-wins / merge). *Optimistic* = detect at save (no locks); *pessimistic* = lock up front.
Related: a **direct DB data change** isn't seen by a loaded context (stale) — use `Reload()` / a fresh context / `AsNoTracking` to refresh.

---

## Q9 — Relationships & conventions
**How EF infers the one-to-many + FK (conventions):**
- **Navigation properties**: a reference (`Product.Category`) on one side + a collection (`Category.Products`) on the other ⇒ **one-to-many**.
- **Foreign key by name**: a property matching `<Nav>Id` / `<PrincipalType>Id` / `<PrincipalType><PrincipalKeyName>` → `Product.CategoryId` maps to `Category.Id`. (No FK property ⇒ EF creates a **shadow** FK.)
- **Primary key**: a property named `Id` or `<Type>Id`.
- **Required vs optional**: non-nullable FK (`int CategoryId`) ⇒ required (cascade delete by convention); nullable (`int?`) ⇒ optional.

**Overriding conventions — two ways:**
1. **Data annotations** — `[Key]`, `[ForeignKey(nameof(Category))]`, `[Required]`, `[Column]` on the model (simple cases).
2. **Fluent API** in `OnModelCreating` (preferred for anything complex; keeps entities POCO):
   ```csharp
   modelBuilder.Entity<Product>()
       .HasOne(p => p.Category)
       .WithMany(c => c.Products)
       .HasForeignKey(p => p.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);
   ```
**Precedence:** Fluent API > Data Annotations > Conventions.

---

## 📊 Overall assessment (EF Core)

**Score: ~5.5–6 / 10** — solid conceptual instincts, gaps in precision and the "advanced" topics.

**Strengths**
- Genuinely good grasp of the core ideas once prompted.
- **Excellent probing questions** (source-of-truth, direct DB data/schema changes, drift) — real engineering curiosity that interviewers value.
- Improved mid-session (started landing recommendations).

**Weak areas to drill**
1. **Name *all* the options**, not just the common one (loading: eager/lazy/explicit/projection).
2. **Always land the fix/recommendation**, not just the problem (N+1).
3. **Precision on mechanics/terminology** — *change tracker* (not "logs"), `IQueryable` has **no caching**, `DbContext` is **not thread-safe**, `DetectChanges`/snapshot.
4. **Advanced topics that were gaps:** migrations, optimistic concurrency (`rowversion`), relationship configuration (conventions + Fluent API).
5. Practice delivering complete **soundbite** answers.

**Next mock:** re-test Q1/Q6 (loading + change tracking) cold, then move to C#/async and ASP.NET Core.
