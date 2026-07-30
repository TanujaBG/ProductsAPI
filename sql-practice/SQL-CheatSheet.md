# SQL — When to Use What (Cheat Sheet)

A decision guide for the techniques from the deep dive. Start with the **mental model**, then the **"I want to…"** table. The rest is quick reference.

---

## The one mental model — logical processing order
```
FROM / JOIN  →  WHERE  →  GROUP BY  →  HAVING  →  SELECT  →  ORDER BY
```
This single order explains most "why doesn't this work?" moments:
- `WHERE` runs **before** `GROUP BY` → can't filter aggregates in `WHERE` (use `HAVING`).
- Aliases are created in `SELECT` → usable only in `ORDER BY`, **not** `WHERE`/`GROUP BY`/`HAVING`.
- Window functions run in `SELECT` → can't filter them in `WHERE` (wrap in a CTE).

---

## "I want to…" → use this
| Goal | Tool |
|---|---|
| Filter individual rows | `WHERE` |
| Filter groups by an aggregate (e.g. total > 500) | `HAVING` |
| Combine two tables | `JOIN … ON` |
| Keep all rows from one side even with no match | `LEFT JOIN` (+ `COALESCE(...,0)`) |
| One summary row per group | `GROUP BY` + `SUM/COUNT/AVG/MIN/MAX` |
| Keep every row **and** show a group-level number | window: `func() OVER (…)` |
| Rank rows (leaderboard, top-N) | `ROW_NUMBER` / `RANK` / `DENSE_RANK` |
| Running total / moving sum | `SUM(x) OVER (ORDER BY …)` |
| Compare a row to the previous / next | `LAG(x)` / `LEAD(x)` |
| Top N **per group** | `ROW_NUMBER()` in a **CTE**, then `WHERE rn <= N` |
| The **row** holding the max (not just the value) | `ROW_NUMBER()` (not `MAX`) |
| Turn rows into columns (e.g. spend per category) | `SUM(CASE WHEN … THEN … ELSE 0 END)` |
| Percent of total | `x * 100.0 / SUM(x) OVER ()` |
| Rows with **no** match ("never ordered") | `NOT EXISTS` or `LEFT JOIN … WHERE key IS NULL` |
| Pair / compare rows in the same table | **self-join** with `a.id < b.id` |
| Name a step / reuse / recursion / readability | **CTE** (`WITH`) |
| A single value inside a condition | **scalar subquery** |
| "Does at least one match exist?" | `EXISTS` |
| Big intermediate reused across statements | **temp table** `#t` |

---

## JOINs at a glance
| Type | Keeps |
|---|---|
| `INNER` | only rows matching on both sides |
| `LEFT` | all left rows (+ matches; NULLs when none) |
| `RIGHT` | all right rows |
| `FULL` | all rows from both sides |
| `CROSS` | every combination (no `ON`) |
| self | a table joined to itself (pairs, hierarchies) |

**Gotcha:** once you `LEFT JOIN`, a later `INNER JOIN` (or a `WHERE` on the right side) silently turns it back into an inner join. Once you go LEFT, stay LEFT.

---

## Ranking functions — ties for values `100, 100, 90`
| Function | Result | Use |
|---|---|---|
| `ROW_NUMBER` | 1, 2, 3 | unique numbering, exact top-N |
| `RANK` | 1, 1, 3 | standings with **gaps** after ties |
| `DENSE_RANK` | 1, 1, 2 | standings, **no gaps** |

---

## GROUP BY vs Window function
- `GROUP BY` **collapses** → one row per group.
- Window (`OVER`) **keeps every row** → detail + group calc side by side.
- Aggregate **once** at the grain you want; don't stack a `GROUP BY` on top of a window.
- `MAX(x)` gives the **value**; `ROW_NUMBER()` gives the **row** that holds it.

---

## Subqueries
| Kind | Returns | Used with |
|---|---|---|
| scalar | 1 value | `WHERE x > (…)` |
| multi-row | a list | `IN (…)` |
| correlated | depends on outer row | `EXISTS (…)` |

Prefer **`NOT EXISTS`** over `NOT IN` — a single NULL in the subquery makes `NOT IN` return **nothing**.

---

## CTE vs Temp table vs Table variable
| | CTE | Table variable `@t` | Temp table `#t` |
|---|---|---|---|
| Is | named query (no data) | rows in a variable | scratch table |
| Scope | one statement | the batch/proc | the session |
| Stats / indexes | none | minimal | full |
| Rolled back by a transaction? | n/a | **no** | yes |
| Recursion | yes | no | no |

---

## NULL rules
- `= NULL` / `!= NULL` → **UNKNOWN** (never true). Use `IS NULL` / `IS NOT NULL`.
- `COUNT(col)` ignores NULLs; `COUNT(*)` counts rows.
- `COALESCE(x, 0)` for a default — but keep NULL when "none" ≠ "zero".

---

## Indexes (quick)
- **Clustered** = the table itself, sorted by key (one per table). **Nonclustered** = a separate lookup structure (many allowed).
- **Covering index:** add `INCLUDE (cols)` so the query needs no key lookup.
- **Sargable** = don't wrap the indexed column in a function: `WHERE OrderDate >= '2026-01-01'` ✅ vs `WHERE YEAR(OrderDate) = 2026` ❌.
- **Composite key order:** equality columns first, then the range column (leftmost-prefix rule).

---

## Transactions & isolation (quick)
- Wrap all-or-nothing work in `BEGIN TRAN … COMMIT` with `TRY/CATCH` + `ROLLBACK` (guard with `IF @@TRANCOUNT > 0`), and `THROW` to re-raise.
- `READ COMMITTED` = default. Anomalies: dirty / non-repeatable / phantom.
- `SERIALIZABLE` **blocks** (range locks); `SNAPSHOT` **versions** (readers don't block writers).

---

## T-SQL gotchas (things that bit us)
- Equality is `=` — there is **no `==`**.
- Strings use **single quotes** `'x'`; double quotes `"x"` mean an **identifier** (a column/table name).
- Can't filter a window function in `WHERE` → compute it in a CTE, filter outside.
- `OFFSET … FETCH` (any pagination) needs an `ORDER BY`.
- Money → `DECIMAL`, never `FLOAT`.
- Anti-join: test a `NOT NULL` / PK column for `IS NULL`.

---

## Classic interview questions & patterns

> Framed with the usual `Employees(Id, Name, DeptId, Salary, ManagerId)` / `Departments(Id, Name)` schema. The **technique** is what matters — it transfers to any tables (product/category = employee/department).

### ⭐ Nth highest salary **per department**
```sql
WITH r AS (
    SELECT DeptId, Name, Salary,
           DENSE_RANK() OVER (PARTITION BY DeptId ORDER BY Salary DESC) AS rnk
    FROM Employees
)
SELECT DeptId, Name, Salary FROM r WHERE rnk = 2;   -- 2nd highest; use 1 for top earner
```
**Key:** `PARTITION BY DeptId` ranks *within* each department; `DENSE_RANK` lets ties share a rank; filter the rank in an **outer** query (you can't filter a window in `WHERE`).

### Nth highest salary — overall
```sql
WITH r AS (
    SELECT Salary, DENSE_RANK() OVER (ORDER BY Salary DESC) AS rnk FROM Employees
)
SELECT DISTINCT Salary FROM r WHERE rnk = 2;                       -- change 2 → N
-- or: SELECT DISTINCT Salary FROM Employees ORDER BY Salary DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY;
```
**Key:** `DENSE_RANK` (ties count once), not `ROW_NUMBER`.

### Highest-paid employee in each department (the *row*, not just the value)
```sql
WITH r AS (
    SELECT DeptId, Name, Salary,
           ROW_NUMBER() OVER (PARTITION BY DeptId ORDER BY Salary DESC) AS rn
    FROM Employees
)
SELECT DeptId, Name, Salary FROM r WHERE rn = 1;
```
**Key:** `MAX(Salary)` gives the value; `ROW_NUMBER()` gives the employee that holds it.

### Find duplicates
```sql
SELECT Email, COUNT(*) AS Cnt FROM Users GROUP BY Email HAVING COUNT(*) > 1;
```

### Delete duplicates, keep one
```sql
WITH d AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY Email ORDER BY Id) AS rn FROM Users
)
DELETE FROM d WHERE rn > 1;      -- keeps the lowest Id per Email
```

### Employees earning more than their manager (self-join)
```sql
SELECT e.Name
FROM Employees e
JOIN Employees m ON m.Id = e.ManagerId
WHERE e.Salary > m.Salary;
```

### Departments with no employees (anti-join)
```sql
SELECT d.Name FROM Departments d
WHERE NOT EXISTS (SELECT 1 FROM Employees e WHERE e.DeptId = d.Id);
```

### Department with the highest average salary
```sql
SELECT TOP 1 WITH TIES DeptId, AVG(Salary) AS AvgSalary
FROM Employees GROUP BY DeptId ORDER BY AvgSalary DESC;
```

### Compare to previous period (month-over-month)
```sql
SELECT Month, Revenue,
       Revenue - LAG(Revenue) OVER (ORDER BY Month) AS MoMChange
FROM MonthlySales;
```

### Median
```sql
SELECT DISTINCT PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY Salary) OVER () AS Median
FROM Employees;
```

### Pivot (rows → columns) with conditional aggregation
```sql
SELECT DeptId,
       SUM(CASE WHEN Gender = 'M' THEN 1 ELSE 0 END) AS Males,
       SUM(CASE WHEN Gender = 'F' THEN 1 ELSE 0 END) AS Females
FROM Employees GROUP BY DeptId;
```

### Consecutive runs / streaks (gaps-and-islands)
```sql
SELECT Val, MIN(TheDate) AS StartDate, MAX(TheDate) AS EndDate
FROM (
    SELECT *, DATEADD(DAY, -ROW_NUMBER() OVER (ORDER BY TheDate), TheDate) AS grp
    FROM Attendance
) x
GROUP BY Val, grp;
```
**Key idea:** within a consecutive run, `date − ROW_NUMBER()` stays **constant**, so you can group by it to collapse each streak.
